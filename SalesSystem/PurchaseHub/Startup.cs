namespace PurchaseHub
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using PurchaseHub.Services;
    using PurchaseHub.Services.Implementations;
    using PurchaseHub.Hubs;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            
            services.AddSingleton<IDatabase>(
                new Database(Configuration["DbConnection:Server"], Configuration["DbConnection:KeySpace"])
            );

            services.AddSingleton<IDbQuery, DbQuery>();
            services.AddSingleton<ISalesQuery, SalesQuery>();
            services.AddSingleton<ISalesLog, SalesLog>();
            services.AddHostedService<DatabaseHostedService>();

            services.AddControllers();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHMACHashing();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SalesHub>("/salesHub");
            });
        }
    }
}
