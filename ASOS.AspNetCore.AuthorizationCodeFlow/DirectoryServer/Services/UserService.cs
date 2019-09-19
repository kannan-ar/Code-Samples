using DirectoryServer.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<UserModel> users;

        public UserService(IDirectoryDbSettings dbSettings, IMongoService mongoService)
        {
            users = mongoService.GetCollection<UserModel>(dbSettings.UserCollectionName);
        }

        public void Add(UserModel model)
        {
            if (users.Find<UserModel>(u=> u.UserName == model.UserName).Any())
            {
                throw new InvalidOperationException("User already exists");
            }

            users.InsertOne(model);
        }
    }
}
