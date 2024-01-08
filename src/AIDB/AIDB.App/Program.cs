using AIDB.App.Configuration;
using AIDB.App.DataSeeder;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var configRef = builder.Configuration;
var servicesRef = builder.Services;

// Set configuration settings
configRef.SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddEnvironmentVariables();

// Configure Logger
Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Async(e => e.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code))
    .WriteTo.Async(e => e.File(Path.Combine(AppContext.BaseDirectory, "Logs/AIDB.log"), restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Hour))
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
);

// Configure services
servicesRef.AddCore()
           .AddBaseServices(configRef)
           .AddHttpClients()
           .AddDatabase(configRef);

// Configure && run the app
var app = builder.Build();
app.ConfigureAppBase()
   .UseAIDBSwagger()
   .Migrate();

app.Map("/api", () => "Hello World!");

await DataSeed.SeedDataAsync(app);
app.Run();
