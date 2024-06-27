using Messaging.Lib;
using Microsoft.EntityFrameworkCore;

namespace ProducerApp
{
    public static class ServiceCollectionExtensions
    {
        public static void InitDatabase(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbcotext = serviceProvider.GetRequiredService<SqlDbContext>();
            dbcotext.Database.Migrate();
        }
    }
}
