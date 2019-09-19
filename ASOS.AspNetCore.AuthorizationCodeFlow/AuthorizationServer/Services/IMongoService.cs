using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Services
{
    public interface IMongoService
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
