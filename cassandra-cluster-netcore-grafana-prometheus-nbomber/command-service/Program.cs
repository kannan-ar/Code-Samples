using Asp.Versioning;
using CommandServiceApi.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

var loggingConfig = builder.Configuration.GetSection("Logging");
var applicationName = loggingConfig.GetValue<string>("Loki:ApplicationName") ?? "UnknownService";

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(applicationName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(builder.Configuration["Tempo:OtlpEndpoint"] 
                    ?? throw new ArgumentNullException("OtlpEndpoint"));
            });
    });

builder.Host.UseSerilog((context, services, configuration) =>
{
    var loggingConfig = context.Configuration.GetSection("Logging");

    configuration
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", applicationName)
        .Enrich.WithSpan()
        .WriteTo.GrafanaLoki(loggingConfig.GetValue<string>("Loki:LokiUrl") 
            ?? throw new ArgumentNullException("LokiUrl"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.ConfigureMassTransit(builder.Configuration);

//builder.Services.AddMetrics();

var app = builder.Build();

app.UseHttpMetrics();
app.MapMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
