using System.Threading.Tasks;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain.Services
{
    public interface IUserService
    {
        public Task<User> GetUserById(int id);
    }
}
