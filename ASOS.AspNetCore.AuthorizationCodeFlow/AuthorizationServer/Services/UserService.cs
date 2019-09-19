using AuthorizationServer.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<UserModel> users;

        public UserService(IDirectoryDbSettings dbSettings, IMongoService mongoService)
        {
            users = mongoService.GetCollection<UserModel>(dbSettings.UserCollectionName);
        }

        public UserModel Get(string userId)
        {
            return users.Find(u => u.UserName == userId).FirstOrDefault();
        }

        public bool HasUser(string clientId, string username, string pwd)
        {
            var user = users.Find(u => u.UserName == username && u.ClientId == clientId).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            return string.Compare(user.Password, pwd, false) == 0;
        }
    }
}
