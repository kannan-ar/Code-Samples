using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Models
{
    public class DirectoryDbSettings : IDirectoryDbSettings
    {
        public string ManagersCollectionName { get; set; }
        public string ClientCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
