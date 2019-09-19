using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public interface IMongoService
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
