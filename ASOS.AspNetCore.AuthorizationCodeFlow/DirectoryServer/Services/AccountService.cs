using DirectoryServer.Models;
using MongoDB.Driver;
using System.Linq;


namespace DirectoryServer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMongoCollection<ManagerModel> managers;

        public AccountService(IDirectoryDbSettings dbSettings, IMongoService mongoService)
        {
            managers = mongoService.GetCollection<ManagerModel>(dbSettings.ManagersCollectionName);
        }

        public bool IsAdmin(string username, string password)
        {
            ManagerModel manager = managers.Find<ManagerModel>(m => m.UserName == username).FirstOrDefault();
            return manager == null ? false : string.Compare(manager.Password, password, false) == 0;
        }
    }
}
