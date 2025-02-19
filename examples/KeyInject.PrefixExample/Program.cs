using KeyInject;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddKeyInject();

var app = builder.Build();

await Console.Out.WriteLineAsync(app.Configuration["key-1"]);
await Console.Out.WriteLineAsync(app.Configuration["key-2"]);
await Console.Out.WriteLineAsync(app.Configuration["key-3"]);
await Console.Out.WriteLineAsync(app.Configuration["key-4"]);
await Console.Out.WriteLineAsync(app.Configuration["key-5"]);
await Console.Out.WriteLineAsync(app.Configuration["key-6"]);
await Console.Out.WriteLineAsync(app.Configuration["key-7"]);

// await app.RunAsync();
