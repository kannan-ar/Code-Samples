using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Models
{
    public interface IDirectoryDbSettings
    {
        string ManagersCollectionName { get; set; }
        string ClientCollectionName { get; set; }
        string UserCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
