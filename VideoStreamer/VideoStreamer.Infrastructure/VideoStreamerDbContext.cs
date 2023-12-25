using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VideoStreamer.Infrastructure.Entities;
using VideoStreamer.Infrastructure.Settings;

namespace VideoStreamer.Infrastructure
{
    public class VideoStreamerDbContext : DbContext
    {
        private readonly DbSettings dbSettings;

        public VideoStreamerDbContext(DbContextOptions<VideoStreamerDbContext> options, IOptions<DbSettings> dbSettings)
            : base(options)
        {
            this.dbSettings = dbSettings.Value;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite(dbSettings.ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(x => new { x.RoleId, x.UserId });
            modelBuilder.Entity<UserRole>().HasOne<Role>(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<UserRole>().HasOne<User>(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
        }
    }
}
