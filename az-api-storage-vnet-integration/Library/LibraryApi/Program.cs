using Azure.Identity;
using LibraryApi.Features.Products;
using LibraryApi.Helpers;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var tableStorage = builder.Configuration.GetValue<string>("TableStorage");

//https://arwebstorage8989.table.core.windows.net

builder.Services.AddAzureClients(configureClients =>
{
    if(builder.Environment.IsDevelopment())
    {
        configureClients.AddTableServiceClient(tableStorage);
    }
    else
    {
        configureClients.AddTableServiceClient(new Uri(tableStorage));
    }

    configureClients.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddSingleton<IStorageClient, StorageClient>();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
