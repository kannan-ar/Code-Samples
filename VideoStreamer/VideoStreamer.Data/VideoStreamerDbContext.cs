using Microsoft.EntityFrameworkCore;
using VideoStreamer.Data.Entities;

namespace VideoStreamer.Data
{
    public class VideoStreamerDbContext : DbContext
    {
        public VideoStreamerDbContext(DbContextOptions<VideoStreamerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(x => new { x.RoleId, x.UserId });
            modelBuilder.Entity<UserRole>().HasOne<Role>(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<UserRole>().HasOne<User>(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
        }
    }
}
