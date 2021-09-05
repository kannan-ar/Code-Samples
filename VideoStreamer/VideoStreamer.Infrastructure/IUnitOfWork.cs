using VideoStreamer.Infrastructure.Entities;
using VideoStreamer.Infrastructure.Repositories;

namespace VideoStreamer.Infrastructure
{
    public interface IUnitOfWork
    {
        GenericRepository<User> UserRepository { get; }
        GenericRepository<Role> RoleRepository { get; }
        void Save();
    }
}
