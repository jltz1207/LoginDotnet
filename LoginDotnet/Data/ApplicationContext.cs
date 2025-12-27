using LoginDotnet.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace LoginDotnet.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserActivityLog>(
                entity =>
                {
                    entity.HasKey(e => new { e.Email,e.ActivityType, e.Timestamp });
                }
            );
        }
    }
}
