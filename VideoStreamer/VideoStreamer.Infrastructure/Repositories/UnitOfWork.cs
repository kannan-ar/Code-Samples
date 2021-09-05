using VideoStreamer.Infrastructure.Entities;

namespace VideoStreamer.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private VideoStreamerDbContext dbContext;

        public UnitOfWork(VideoStreamerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public GenericRepository<User> UserRepository => new GenericRepository<User>(dbContext);
        public GenericRepository<Role> RoleRepository => new GenericRepository<Role>(dbContext);

        public void Save()
        {
            dbContext.SaveChanges();
        }
    }
}
