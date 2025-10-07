using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YAPA.Models;
using YAPA.Models.Entities;

namespace YAPA.Db
{
    public class AppDbContext : IdentityDbContext<UserModel, IdentityRole<int>,int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<PomodoroModel> Pomodoros { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>()
                .HasMany<RefreshTokenModel>()
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
