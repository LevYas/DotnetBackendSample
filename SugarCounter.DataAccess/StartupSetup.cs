using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SugarCounter.Core.Food;
using SugarCounter.Core.Sessions;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db;
using SugarCounter.DataAccess.Repositories;

namespace SugarCounter.DataAccess
{
    public static class StartupSetup
    {
        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ISessionsRepository, SessionsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
