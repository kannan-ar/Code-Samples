using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer.Models
{
    public class IdentityConfiguration
    {
        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "8989",
                    Username = "ar",
                    Password = "123",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Kannan Ramachandran"),
                        new Claim(JwtClaimTypes.GivenName, "Kannan"),
                        new Claim(JwtClaimTypes.FamilyName, "Kannan"),
                        new Claim(JwtClaimTypes.WebSite, "http://kannanar.com"),
                    }
                }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("myApi.read"),
                new ApiScope("myApi.write"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("myApi")
                {
                    Scopes = new List<string>{ "myApi.read","myApi.write" },
                    ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "cwm.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "myApi.read" }
                },
                new Client
                {
                    ClientId = "swaggerui",
                    ClientName = "swaggerui",
                    AllowedGrantTypes = GrantTypes.Implicit,
                     ClientSecrets = {new Secret("secret".Sha256())},
                    RequireConsent = false,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowPlainTextPkce = false,
                    RedirectUris = { "https://localhost:8000/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:8000/swagger/o2c.html" },
                    AllowedCorsOrigins = { "https://localhost:8000" },
                    AllowedScopes = { "myApi.read" }
                },
                new Client
                {
                    ClientId ="shellclient",
                    RequireClientSecret = false,
                    ClientName = "shellclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowPlainTextPkce = false,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "https://localhost:7000/callback" },
                    PostLogoutRedirectUris = new [] { "https://localhost:7000" },
                    AllowedCorsOrigins = { "myApi.read" }
                }
            };
    }
}
