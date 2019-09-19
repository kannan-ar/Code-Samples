using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using MongoDB.Driver;

namespace AuthorizationServer.Services
{
    public class ClientService : IClientService
    {
        private readonly IMongoCollection<ClientModel> clients;

        public ClientService(IDirectoryDbSettings dbSettings, IMongoService mongoService)
        {
            clients = mongoService.GetCollection<ClientModel>(dbSettings.ClientCollectionName);
        }
        public ClientModel Get(string clientId)
        {
            return clients.Find(c => c.ClientId == clientId).FirstOrDefault();
        }
    }
}
