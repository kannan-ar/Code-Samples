namespace SalesSimulator.Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;

    using Models;
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly Random random;
        public UserService(IConfiguration configuration)
        {
            this.configuration = configuration;
            random = new Random();
        }
        public async Task<IList<User>> GetUsers()
        {
            var usersApi = configuration.GetSection("APIUrls")["UsersAPI"];
            var httpClient = new HttpClient();

            if (string.IsNullOrWhiteSpace(usersApi))
            {
                throw new ArgumentNullException("Users API url should not be empty");
            }

            return await JsonSerializer.DeserializeAsync<IList<User>>(await httpClient.GetStreamAsync(usersApi));
        }

        public IEnumerable<User> GetActiveUsersFromCollection(IList<User> users)
        {
            var activeCount = random.Next(1, users.Count);

            for(int i = 0; activeCount> i; i++)
            {
                yield return users[random.Next(0, users.Count-1)];
            }
        }

        public IEnumerable<string> GetCurrentInterests(User user)
        {
            if(user == null || user.Interests == null || user.Interests.Length == 0)
            {
                yield break;
            }

            var currentCount = random.Next(1, user.Interests.Length);

            for(int i = 0; currentCount> i; i++)
            {
                yield return user.Interests[random.Next(0, user.Interests.Length-1)];
            }
        }
    }
}