using Azure.Identity;
using MediaServices.MediaViewer.Services;
using MediaServices.MediaViewer.Settings;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"));
builder.Services.AddSingleton<IFileProvider>(
       new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddAzureClients(configureClients =>
{
    configureClients.AddBlobServiceClient(builder.Configuration.GetValue<string>("BlobEndpointUrl"));
    configureClients.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddSingleton<IBlobManager, BlobManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
