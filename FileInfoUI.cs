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
                    "_Open",
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
