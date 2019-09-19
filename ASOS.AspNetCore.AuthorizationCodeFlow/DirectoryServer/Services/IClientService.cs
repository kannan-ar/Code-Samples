using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectoryServer.Models;

namespace DirectoryServer.Services
{
    public interface IClientService
    {
        void Add(ClientModel model);
        List<ClientModel> GetAll();
    }
}
