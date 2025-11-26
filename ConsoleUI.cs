// https://github.com/gui-cs/Terminal.Gui/tree/v1_release
#:package Terminal.Gui@1.19.0
#:package MimeMapping@3.1.0
using System.IO;
using System.Linq;
using Terminal.Gui;

Application.Init();

var win = new Window("Console UI")
{
    X = 0,
    Y = 1,
    Width = Dim.Fill(),
    Height = Dim.Fill()
};
Application.Top.Add(win);

var menu = new MenuBar(
    [
        new MenuBarItem(
            "_File",
            [
                new MenuItem("_Get File Info", string.Empty, ShowGetFileInfoPrompt),
                new MenuItem("_New Entry (Dialog Form)", string.Empty, ShowNewEntryFormDialog),
                new MenuItem("_Exit", string.Empty, ShowExitPrompt)
            ]
        )
    ]
);
Application.Top.Add(menu);

Application.Run();
Application.Shutdown();

static void ShowExitPrompt()
{
    var result = MessageBox.Query(
        "Exit",
        "Are you sure you want to exit?",
        "Yes",
        "No"
    );

    if (result != 0)
    {
        return;
    }

    Application.RequestStop();
}

static void ShowGetFileInfoPrompt()
{
    var openDialog = new OpenDialog(
        "Open File",
        "Select a file"
    )
    {
        AllowsMultipleSelection = false
    };

    Application.Run(openDialog);

    if (
        openDialog.Canceled
        || openDialog.FilePath.ToString() is not { Length: > 0 } filePath
    )
    {
        return;
    }

    var details = new FileInfo(filePath) switch
    {
        { Exists: true } fileInfo => $"""
Size: {fileInfo.Length} bytes
Created: {fileInfo.CreationTime}
Last Modified: {fileInfo.LastWriteTime}
MIME Type: {MimeMapping.MimeUtility.GetMimeMapping(filePath)}
Lines: {File.ReadLines(filePath).Count()}
""",
        _ => $"The specified file/path '{filePath}' does not exist..."
    };

    MessageBox.Query(
        "File Selected",
$"""
You selected: {filePath}

{details}

""",
        "Ok"
    );
}

static void ShowNewEntryFormDialog()
{
    var form = new Dialog("New Entry", 60, 13);

    var nameLbl = new Label(1, 1, "Name: ");
    var nameField = new TextField("") { X = 14, Y = 1, Width = 40 };

    var lastLbl = new Label(1, 3, "Last name: ");
    var lastField = new TextField("") { X = 14, Y = 3, Width = 40 };

    var ageLbl = new Label(1, 5, "Age (years): ");
    var ageField = new TextField("") { X = 14, Y = 5, Width = 8 };

    var submitBtn = new Button("Submit") { X = Pos.Center() - 10, Y = 8 };
    var cancelBtn = new Button("Cancel") { X = Pos.Center() + 2, Y = 8 };

    var entryDetailsSummary = string.Empty;

    nameField.KeyPress += OnTextInput;
    lastField.KeyPress += OnTextInput;
    ageField.KeyPress += OnTextInput;
    submitBtn.Clicked += () => Submit();
    cancelBtn.Clicked += () => Application.RequestStop(form);

    form.Add(nameLbl, nameField, lastLbl, lastField, ageLbl, ageField, submitBtn, cancelBtn);

    Application.Run(form);

    if (entryDetailsSummary.Length == 0)
    {
        return;
    }

    MessageBox.Query("Entry Details", entryDetailsSummary, "Ok");

    void OnTextInput(View.KeyEventEventArgs args)
    {
         if (args.KeyEvent.Key == Key.Enter)
        {
            Submit();
            args.Handled = true;
        }
    }

    void Submit()
    {
        if (nameField.Text?.ToString()?.Trim() is not { Length: >= 2 and <= 32 } name)
        {
            MessageBox.ErrorQuery(
                "Validation",
                "Please enter a name greater or equal to 2 and less than or equal to 32 characters.",
                "Ok"
            );
            return;
        }

        if (lastField.Text?.ToString()?.Trim() is not { Length: >= 2 and <= 32 } lastname)
        {
            MessageBox.ErrorQuery(
                "Validation",
                "Please enter a last name greater or equal to 2 and less than or equal to 32 characters.",
                "Ok"
            );
            return;
        }

        if (
            ageField.Text?.ToString()?.Trim() is not { Length: > 0 } ageText
            || !int.TryParse(ageText, out var age)
            || age <= 0
            || age > 120
        )
        {
            MessageBox.ErrorQuery(
                "Validation",
                "Age must be a whole number greater than 0 and less than or equal to 120.",
                "Ok"
            );
            return;
        }

        entryDetailsSummary =
$"""
Name: {name}
Last name: {lastname}
Age: {age} year(s) old
""";

        Application.RequestStop(form);
    }
}
