using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Utils;
using System;
using System.Linq;

namespace SugarCounter.DataAccess.Db
{
    // In real-life appplication I would use EF Core migration feature
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider services)
        {
            try
            {
                Initialize(services.GetRequiredService<AppDbContext>());
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DbInitializer>>();
                logger.LogError(ex, "An error occurred while initializing the database.");
            }
        }

        internal static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            // It's better to get password from protected storage
            context.Users.Add(new UserInfo { Login = "root", PasswordHash = "Admin".MakeSha256String(), Role = UserRole.Admin });

            context.SaveChanges();
        }
    }
}
