using KeyInject;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddKeyInject(b => b
	// simply enable or disable globally 
	.SetEnabled(true)
	// adding custom prefixes
	.AddKeyPrefix("PRE_")
	.AddKeyPrefix("DATABASE_")
	// adding custom regex pattern. Warn! Must to use ?<key> regex group, see documentation.
	.AddRegexPattern(@"!\{(?<key>[^{}]+)\}!")
	// set how many time config will be injected to resolve circular dependencies
	.SetReplaceRepeatCount(10)
	// ignore case of pattern key group >> ${IgNore_Case_Of_thIs_woRD}
	.SetIgnoreCase(true)
	// choose yor custom config section instead default "KeyInject", first way:
	.EnrichFromAppSettings(builder.Configuration.GetSection("MyCustomSection"))
	// second way:
	.EnrichFromAppSettings(c => c.GetSection("MyCustomSection"))
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();