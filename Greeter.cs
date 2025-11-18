#:package Spectre.Console@0.54.0
using Spectre.Console;

var _nl = Environment.NewLine;

AnsiConsole.MarkupLine(
    $"[steelblue1]Hello[/] [navy]and[/] [teal]welcome[/] [steelblue1]to[/] [navy]Greeter![/]{_nl}"
);

var name = string.Empty;
string[] questionColors =
[
    "darkseagreen3",
    "darkorange3",
    "purple"
];

for (var i = 0; i < questionColors.Length; i++)
{
    var color = questionColors[i];
    AnsiConsole.Markup($"[{color}]Please enter your name: [/]");
    name = Console.ReadLine()?.Trim() ?? string.Empty;

    if (name.Length > 0)
    {
        break;
    }
}

var result = name.Length switch
{
    > 0 => $"{_nl}[yellow]Nice to meet you, [bold]{name}[/]![/]",
    _ => $"{_nl}[silver]Well.. I guess without a name a greeting cannot take place...[/]"
};

AnsiConsole.MarkupLine(result);