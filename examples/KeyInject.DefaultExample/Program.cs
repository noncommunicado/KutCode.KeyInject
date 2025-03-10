using KeyInject;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false, true);
// in development, from launchSettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddKeyInject(
    kb => kb
        .SetReloadEnabled(true)
        .SetIgnoreCase(true),
    LoggerFactory.Create(b => b
        .SetMinimumLevel(LogLevel.Debug)
        .AddConsole()
        .AddDebug())
);

var app = builder.Build();

// output: server=1.4.8.8;user=root-user;password=12345qwe_dontdothat
await Console.Out.WriteLineAsync(app.Configuration.GetConnectionString("Main"));
app.MapGet("/conn", async (IConfiguration config) => config.GetConnectionString("Main"));

app.Run();