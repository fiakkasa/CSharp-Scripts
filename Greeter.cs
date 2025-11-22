#:package Spectre.Console@0.54.0
using Spectre.Console;

const int _minNameLength = 2;

AnsiConsole.MarkupLine(
    "[steelblue1]Hello[/] [navy]and[/] [teal]welcome[/] [steelblue1]to[/] [navy]Greeter![/]"
);

var name = string.Empty;
(string, string)[] questionColors =
[
    ("darkseagreen3", "seagreen3"),
    ("darkorange3", "orange1"),
    ("purple", "mediumpurple")
];

for (var i = 0; i < questionColors.Length; i++)
{
    Console.Clear();

    var (promptColor, validationColor) = questionColors[i];

    if (i > 0)
    {
        AnsiConsole.MarkupLine(
            $"[{validationColor}]Consider using 2 or more characters for your name...[/]"
        );
    }

    name = AnsiConsole.Prompt(
        new TextPrompt<string>($"[{promptColor}]Please enter your name: [/]")
            .AllowEmpty()
    );

    if (name.Length >= _minNameLength)
    {
        break;
    }
}

Console.Clear();

var result = name.Length switch
{
    >= _minNameLength => $"[yellow]Nice to meet you, [bold]{name}[/]![/]",
    _ => """ 
[silver]Well.. I guess without a valid name a greeting cannot take place...[/]

[grey93]Nice to meet you anyway :folded_hands:[/]
"""
};
var panel = new Panel(result)
{
    Border = BoxBorder.Rounded,
    Padding = new Padding(3, 1, 3, 1)
};

AnsiConsole.Write(panel);