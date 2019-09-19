using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectoryServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DirectoryServer.Models;
using Microsoft.Extensions.Options;

namespace DirectoryServer
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/account/login";
                });

            /*services.AddDistributedRedisCache(option =>
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
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
