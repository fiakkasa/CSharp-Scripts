// https://github.com/gui-cs/Terminal.Gui/tree/v1_release
#:package Terminal.Gui@1.19.0
#:package MimeMapping@3.1.0
using System.IO;
using System.Linq;
using Terminal.Gui;

Application.Init();

var win = new Window("File Info (UI)")
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
                new MenuItem(
                    "_Get File Info",
                    string.Empty,
                    () =>
                    {
                        var openDialog = new OpenDialog(
                            "Open File",
                            "Select a file"
                        )
                        {
                            AllowsMultipleSelection = false
                        };

                        Application.Run(openDialog);

                        if (!openDialog.Canceled)
                        {
                            var filePath = openDialog.FilePath.ToString();

                            MessageBox.Query(
                                "File Selected",
                                $"You selected: {filePath}\n\n{GetFileInfo(filePath)}\n\n",
                                "Ok"
                            );
                        }
                    }
                ),
                // New "New Person" form menu item
                new MenuItem(
                    "_New Person (Dialog)",
                    string.Empty,
                    () =>
                    {
                        var form = new Dialog("New Person", 60, 13);

                        var nameLbl = new Label(1, 1, "Name: ");
                        var nameField = new TextField("") { X = 14, Y = 1, Width = 40 };

                        var lastLbl = new Label(1, 3, "Last name: ");
                        var lastField = new TextField("") { X = 14, Y = 3, Width = 40 };

                        var ageLbl = new Label(1, 5, "Age (years): ");
                        var ageField = new TextField("") { X = 14, Y = 5, Width = 8 };

                        var saveBtn = new Button("Save") { X = Pos.Center() - 10, Y = 8 };
                        var cancelBtn = new Button("Cancel") { X = Pos.Center() + 2, Y = 8 };

                        saveBtn.Clicked += () =>
                        {
                            if (nameField.Text.ToString().Trim() is not { Length: >= 2 } name)
                            {
                                MessageBox.ErrorQuery("Validation", "Please enter a name greater or equal to 2 characters.", "Ok");
                                return;
                            }

                            if (lastField.Text.ToString().Trim() is not { Length: >= 2 } lastname)
                            {
                                MessageBox.ErrorQuery("Validation", "Please enter a last name greater or equal to 2 characters.", "Ok");
                                return;
                            }

                            if (
                                ageField.Text.ToString().Trim() is not { Length: > 0 } ageText
                                || !int.TryParse(ageText, out var age)
                                || age <= 0
                                || age > 120
                            )
                            {
                                MessageBox.ErrorQuery(
                                    "Validation",
                                    "Age must be a number greater than 0 and less than or equal to 120.",
                                    "Ok"
                                );
                                return;
                            }

                            MessageBox.Query(
                                "Person Saved",
                                $"Name: {name}\nLastname: {lastname}\nAge: {age}",
                                "Ok"
                            );

                            Application.RequestStop(form);
                        };

                        cancelBtn.Clicked += () => Application.RequestStop(form);

                        form.Add(nameLbl, nameField, lastLbl, lastField, ageLbl, ageField, saveBtn, cancelBtn);

                        Application.Run(form);
                    }
                ),
                new MenuItem("_Exit", string.Empty, () => Application.RequestStop())
            ]
        )
    ]
);
Application.Top.Add(menu);

Application.Run();
Application.Shutdown();

static string GetFileInfo(string filePath) =>
    new FileInfo(filePath) switch
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
