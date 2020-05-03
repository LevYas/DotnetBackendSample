using Microsoft.EntityFrameworkCore;
using SugarCounter.Core.Food;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db.Entities;
using System.Reflection;

namespace SugarCounter.DataAccess.Db
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<UserSession>()
                .HasIndex(u => u.UserInfoId)
                .IsUnique();
        }

        public DbSet<UserInfo> Users { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<FoodItem> FoodItems { get; set; } = null!;
    }
}
