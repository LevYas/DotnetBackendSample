using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SugarCounter.Api;
using SugarCounter.Core.Food;
using SugarCounter.Core.Sessions;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db;
using SugarCounter.DataAccess.Repositories;

namespace Integration
{
    public abstract class IntegrationTestsBase
    {
        internal AppDbContext DbContext;

        protected IntegrationTestsBase()
        {
            var options = createNewContextOptions();
            DbContext = new AppDbContext(options);
            DbInitializer.Initialize(DbContext);
        }

        protected IUsersRepository GetRepository() => new UsersRepository(DbContext);
        protected ISessionsRepository GetAuthRepository() => new SessionsRepository(DbContext);
        protected IFoodRepository GetFoodRepository() => new FoodRepository(DbContext);

        protected RequestContext GetContextForAdmin()
        {
            return new RequestContext
            {
                CurrentUser = new UserInfo { Id = 1, Role = UserRole.Admin }
            };
        }

        private static DbContextOptions<AppDbContext> createNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("TestBase")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
