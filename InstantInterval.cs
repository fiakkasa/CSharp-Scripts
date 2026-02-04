#:package System.Reactive@6.1.0
#:package Spectre.Console@0.54.0

using System.Reactive.Linq;
using Spectre.Console;

var taskCompletionSource = new TaskCompletionSource<bool>();
var duration = TimeSpan.FromSeconds(3);
var iterations = 10;

Console.CancelKeyPress += CancelKeyPressHandler;

HeaderPrint();

using var subscription = Observable.Interval(duration)
    .StartWith(-1) // Emit immediately
    .Take(iterations + 1) // Stop after specified iterations + 1
    .Subscribe(IterationPrint, Exit);

taskCompletionSource.Task.Wait();
Console.CancelKeyPress -= CancelKeyPressHandler;

void HeaderPrint()
{
    var header = $"""

[green]Emits a value every {duration.TotalSeconds} seconds, starting immediately![/]

[orange1]Press CTRL + C to exit...[/]

""";
    AnsiConsole.Write(new Panel(header)
    {
        Border = BoxBorder.Rounded,
        Padding = new Padding(2, 2, 2, 2),
        Expand = true,
        BorderStyle = new Style(foreground: Color.Green)
    });
}

void CancelKeyPressHandler(object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine();
    Exit();
}

void Exit()
{
    AnsiConsole.Write(new Panel("[green]Exiting... Goodbye![/]")
    {
        Border = BoxBorder.Rounded,
        Padding = new Padding(2, 2, 2, 2),
        Expand = true,
        BorderStyle = new Style(foreground: Color.Green)
    });
    taskCompletionSource.SetResult(true);
}

void IterationPrint(long value)
{
    var tick = value + 1;
    var (color, borderColor) = (tick % 2) switch
    {
        0 => ("mediumorchid1", Color.MediumOrchid1),
        _ => ("deepskyblue1", Color.DeepSkyBlue1)
    };
    AnsiConsole.Write(new Panel($"[{color}]Tick: {tick}, after {tick * duration.TotalSeconds} seconds at {DateTimeOffset.Now:O}[/]")
    {
        Border = BoxBorder.Rounded,
        Padding = new Padding(2, 2, 2, 2),
        Expand = true,
        BorderStyle = new Style(foreground: borderColor)
    });
}