Console.WriteLine("Welcome to File Info reader!");

Console.Write("Please enter the path to the file you want to read: ");

var filePath = Console.ReadLine()?.Trim() ?? string.Empty;

if (filePath.Length == 0)
{
    Console.WriteLine("No file path provided. Exiting...");
    return;
}

var fileInfo = new FileInfo(filePath);

if (fileInfo.Exists)
{
    Console.WriteLine(
$"""
File Information for: {fileInfo.FullName}
- Size: {fileInfo.Length} bytes
- Created: {fileInfo.CreationTime}
- Last Modified: {fileInfo.LastWriteTime}
"""
);
    return;
}

Console.WriteLine($"The specified file/path '{filePath}' does not exist...");
