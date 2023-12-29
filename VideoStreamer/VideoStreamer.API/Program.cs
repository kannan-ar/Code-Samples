using Serilog;
using Unity.Microsoft.DependencyInjection;
using VideoStreamer.API.Extensions;
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

//builder.Services.ConfigureAuthorization();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(setup =>
    {
        setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Streamer API v1");
        setup.OAuthClientId("streamer_swagger_client");
        setup.OAuthAppName("Video Streamer App");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
