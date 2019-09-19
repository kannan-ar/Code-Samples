using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;

namespace AuthorizationServer.Services
{
    public interface IClientService
    {
        ClientModel Get(string clientId);
    }
}
