using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;

namespace AuthorizationServer.Services
{
    public interface IUserService
    {
        UserModel Get(string userId);
        bool HasUser(string clientId, string username, string pwd);
    }
}
