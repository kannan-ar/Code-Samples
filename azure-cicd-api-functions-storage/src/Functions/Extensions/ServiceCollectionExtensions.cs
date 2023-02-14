using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static bool IsDevelopmentEnvironment()
        {
            return "Development".Equals(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase);
        }

        public static void AddConfiguration(this IConfigurationBuilder builder)
        {
            string basePath = IsDevelopmentEnvironment() ?
                Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") :
                $"{Environment.GetEnvironmentVariable("HOME")}\\site\\wwwroot";

            var environmentName = Environment.GetEnvironmentVariable("EnvironmentName") ?? "";

            builder
                .SetBasePath(basePath)
                .AddJsonFile("Queues.json")
                .AddJsonFile("Tables.json")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
