using DirectoryServer.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public class ClientService : IClientService
    {
        private readonly IMongoCollection<ClientModel> clients;

        public ClientService(IDirectoryDbSettings dbSettings, IMongoService mongoService)
        {
            clients = mongoService.GetCollection<ClientModel>(dbSettings.ClientCollectionName);
        }

        public void Add(ClientModel model)
        {
            if(clients.Find<ClientModel>(c => c.ClientId == model.ClientId).Any())
            {
                throw new InvalidOperationException("Client already exists");
            }

            clients.InsertOne(model);
        }

        public List<ClientModel> GetAll()
        {
            return clients.Find<ClientModel>(c => true).ToList();
        }
    }
}
