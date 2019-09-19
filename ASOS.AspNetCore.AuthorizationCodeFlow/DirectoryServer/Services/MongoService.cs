using DirectoryServer.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public class MongoService : IMongoService
    {
        private readonly IMongoDatabase database;

        public MongoService(IDirectoryDbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            database = client.GetDatabase(dbSettings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }
    }
}
