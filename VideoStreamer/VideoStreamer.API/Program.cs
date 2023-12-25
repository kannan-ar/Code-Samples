using Serilog;
using Unity.Microsoft.DependencyInjection;
using VideoStreamer.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
       .ReadFrom.Configuration(builder.Configuration.GetSection("Logging"))
       .CreateLogger();

builder.Host.UseSerilog();

var plugin = new Plugin();
builder.Configuration.GetSection("Plugin").Bind(plugin);

var unityContainer = builder.Services.Bootstrap(builder.Configuration, plugin);
builder.Host.UseUnityServiceProvider(unityContainer);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
