using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public interface IAccountService
    {
        bool IsAdmin(string username, string password);
    }
}
