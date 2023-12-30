using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using VideoStreamer.API.Helpers;

namespace VideoStreamer.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MetadataAddress = string.Concat(configuration.GetValue<string>("AuthSettings:Address") ?? string.Empty, ".well-known/openid-configuration");
                options.RequireHttpsMetadata = configuration.GetValue<string>("AuthSettings:RequireHttps") == "true";
                options.Audience = configuration.GetValue<string>("AuthSettings:ClientId") ?? string.Empty;
            });
        }

        public static void ConfigureAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var roles = (configuration.GetValue<string>("RoleSettings:DefaultRole") ?? string.Empty).Split(',');

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                            .RequireAuthenticatedUser()
                                            .RequireRole(roles)
                                            .Build();
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformer>();
        }

        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSwaggerGen(options =>
            //{
            //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Enter 'Bearer {token}' here",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        Reference = new OpenApiReference
            //        {
            //            Type = ReferenceType.SecurityScheme,
            //            Id = "Bearer"
            //        }
            //    });

            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                }
            //            },
            //            new string[] { }
            //        }
            //    });
            //});

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Streamer API",
                    Version = "v1",
                    Description = "Streamer API"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri(string.Concat(configuration.GetValue<string>("AuthSettings:Address") ?? string.Empty, "protocol/openid-connect/auth")),
                            TokenUrl = new Uri(string.Concat(configuration.GetValue<string>("AuthSettings:Address") ?? string.Empty, "protocol/openid-connect/token")),
                            Scopes = new Dictionary<string, string>()
                            {
                                { "streamer-swagger-scope", "Streamer Scope" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }
    }
}
