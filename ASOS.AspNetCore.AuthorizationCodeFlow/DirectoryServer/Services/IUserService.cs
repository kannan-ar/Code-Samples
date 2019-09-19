using DirectoryServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public interface IUserService
    {
        void Add(UserModel model);
    }
}
