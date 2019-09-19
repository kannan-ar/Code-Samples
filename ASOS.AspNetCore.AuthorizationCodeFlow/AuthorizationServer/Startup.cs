using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using AuthorizationServer.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using AuthorizationServer.Models;
using Microsoft.Extensions.Options;
using AuthorizationServer.Services;

namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DirectoryDbSettings>(Configuration.GetSection(nameof(DirectoryDbSettings)));
            services.AddSingleton<IDirectoryDbSettings>(sp =>
            sp.GetRequiredService<IOptions<DirectoryDbSettings>>().Value);

            services.AddMvc();
            services.AddAuthentication()
            .AddOpenIdConnectServer(options =>
            {
                options.Provider = new AuthorizationProvider();
                options.AuthorizationEndpointPath = "/connect/authorize";
                options.TokenEndpointPath = "/connect/token";
                options.AllowInsecureHttp = true;
            });
            /*
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["ExternalConnections:RedisConnection"];
                option.InstanceName = "master";
            });*/

            ServiceDependencyResolver.Resolve(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseWelcomePage();
        }
    }
}
