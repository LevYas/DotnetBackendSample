using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SugarCounter.DataAccess.Db;

namespace SugarCounter.DataAccess
{
    public static class StartupSetup
    {
        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services, string connectionString)
        {
            return services.Scan(s => s
                    .FromAssemblyOf<Dummy>() // FromExecutingAssembly assembly doesn't always work well, especially in tests
                    .AddClasses()
                    .AsMatchingInterface())
                .AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        }

        private abstract class Dummy {}
    }
}
