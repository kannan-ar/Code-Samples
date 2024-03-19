using Azure.Identity;
using MediaServices.MediaManager.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAzureClients(configureClients =>
{
    configureClients.AddBlobServiceClient(builder.Configuration.GetValue<string>("BlobEndpointUrl"));
    configureClients.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddSingleton<IBlobManager, BlobManager>();
builder.Services.AddSingleton<IConverter, Converter>();
builder.Services.AddSingleton<ITranscoder, Transcoder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
