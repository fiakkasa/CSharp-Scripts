// Notes:
// to test start mongo container with:
//  arm64v8/mongo image
//      <orchestrator> run -d -p <port>:<port> --name <container_name> -e MONGO_INITDB_ROOT_USERNAME=<username> -e MONGO_INITDB_ROOT_PASSWORD=<password> arm64v8/mongo
//      ex. container run -d -p 27017:27017 --name mongo-db -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=secret arm64v8/mongo
//  x64 image
//      <orchestrator> run -d -p <port>:<port> --name <container_name> -e MONGO_INITDB_ROOT_USERNAME=<username> -e MONGO_INITDB_ROOT_PASSWORD=<password> mongo
//      ex. container run -d -p 27017:27017 --name mongo-db -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=secret mongo
//
// then connect with connection string:
//   mongodb://<username>:<password>@<host>:<port>/?authSource=admin
//   ex. mongodb://root:secret@localhost:27017/?authSource=admin
#pragma warning disable IL3050
#:package Spectre.Console@0.54.0
#:package MongoDB.Driver@3.5.0
using MongoDB.Driver;
using Spectre.Console;

AnsiConsole.MarkupLine("[steelblue1]Welcome to mongo connect![/]");

AnsiConsole.Markup("[darkseagreen3]Please provide a mongo connection string: [/]");
var connectionString = Console.ReadLine()?.Trim() ?? string.Empty;

AnsiConsole.Markup("[darkseagreen3]Please provide the database name: [/]");
var databaseName = Console.ReadLine()?.Trim() ?? string.Empty;

AnsiConsole.MarkupLine("[steelblue1]Connecting..[/]");

var mongoConnect = new MongoConnect(connectionString, databaseName);
var database = mongoConnect.Connect();

if (database == null)
{
    return;
}

AnsiConsole.MarkupLine("[steelblue1]Fetching Collections..[/]");

var collectionNames = new List<string>();

try
{
    collectionNames.AddRange(
        database.ListCollectionNames().ToList()
    );
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine("[red]Failed to fetch collection names..[/]");
    AnsiConsole.WriteException(ex);
    return;
}

if (collectionNames.Count == 0)
{
    AnsiConsole.MarkupLine("[red]No collections found in the database..[/]");
    return;
}

AnsiConsole.MarkupLine("[steelblue1]Generating Stats..[/]");

Color[] colors =
[
    Color.Green,
    Color.Blue,
    Color.Yellow,
    Color.Red,
    Color.Purple,
    Color.Orange1,
    Color.Aqua,
    Color.Fuchsia
];
var items =
    collectionNames
        .Select(
            (name, index) =>
            (
                Label: name,
                Value: database.GetCollection<object>(name).EstimatedDocumentCount(),
                Color: colors[index % colors.Length]
            )
        )
        .ToList();

AnsiConsole.Write(
    new BarChart()
        .Width(60)
        .Label($"[green bold underline] {collectionNames.Count} Collection(s) and Estimated Documents[/]")
        .CenterLabel()
        .AddItems(
            items,
            (item) => new BarChartItem(
                item.Label,
                item.Value,
                item.Color
            )
        )
);

public class MongoConnect(string connectionString, string databaseName)
{
    private readonly string _connectionString = connectionString;
    private readonly string _databaseName = databaseName;

    public IMongoDatabase? Connect()
    {
        try
        {
            if (
                string.IsNullOrWhiteSpace(_connectionString)
                || string.IsNullOrWhiteSpace(_databaseName)
            )
            {
                AnsiConsole.MarkupLine(
                    "[red]Connection string and database name must be provided..[/]"
                );
                return null;
            }

            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);

            AnsiConsole.MarkupLine(
                "[green]Successfully acquired connection details to the MongoDB database![/]"
            );

            return database;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(
                $"[red]Failed to acquire connection details for the MongoDB database..[/]"
            );
            AnsiConsole.WriteException(ex);
            return null;
        }
    }
}
