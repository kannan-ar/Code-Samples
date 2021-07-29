namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;
    public interface IUserService
    {
        Task<IList<User>> GetUsers();
        IEnumerable<User> GetActiveUsersFromCollection(IList<User> users);
        IEnumerable<string> GetCurrentInterests(User user);
        bool Think(User user);
    }
}