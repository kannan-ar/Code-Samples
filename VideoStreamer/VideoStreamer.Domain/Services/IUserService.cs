using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain.Services
{
    public interface IUserService
    {
        public User GetUserById(int id);
    }
}
