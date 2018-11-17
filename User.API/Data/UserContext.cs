using Microsoft.EntityFrameworkCore;
using User.API.Models;

namespace User.API.Data
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> option):base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().ToTable("Users").HasKey(u => u.Id);

            builder.Entity<UserProperty>().ToTable("UserProperties").HasKey(u => new {u.Key, u.AppUserId, u.Value});
            builder.Entity<UserProperty>().Property(u => u.Key).HasMaxLength(100);
            builder.Entity<UserProperty>().Property(u => u.Value).HasMaxLength(100);

            builder.Entity<UserTag>().ToTable("UserTags").HasKey(u => new {u.UserId, u.Tag});
            builder.Entity<UserTag>().Property(u => u.Tag).HasMaxLength(255);

            builder.Entity<BpFile>().ToTable("BpFiles").HasKey(b => b.Id);

            base.OnModelCreating(builder);
        }


    }
}