using IdentityServer.Models;

//https://social.technet.microsoft.com/wiki/contents/articles/37169.net-core-secure-your-web-applications-using-identityserver-4.aspx#Add_IdentityServer_4_references_into_the_token_server
//https://github.com/IdentityServer/IdentityServer4/tree/main/samples/Quickstarts/3_AspNetCoreAndApis

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentityServer(options =>
{
    options.UserInteraction.ErrorUrl = "/Account/Error";
})
.AddInMemoryClients(IdentityConfiguration.Clients)
.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
.AddInMemoryApiResources(IdentityConfiguration.ApiResources)
.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
.AddTestUsers(IdentityConfiguration.TestUsers)
.AddDeveloperSigningCredential();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();
app.UseIdentityServer();
app.UseRouting();
app.MapDefaultControllerRoute();
app.MapControllers();

app.Run();