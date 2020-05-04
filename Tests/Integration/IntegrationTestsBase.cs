using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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
            DbContext = new AppDbContext(createNewContextOptions());
            DbInitializer.Initialize(DbContext);
            UsersRepo = new UsersRepository(DbContext, new NullLogger<UsersRepository>());
            AuthRepo = new SessionsRepository(DbContext);
            FoodRepo = new FoodRepository(DbContext, new NullLogger<FoodRepository>());
        }

        protected IUsersRepository UsersRepo { get; }
        protected ISessionsRepository AuthRepo { get; }
        protected IFoodRepository FoodRepo { get; }

        protected RequestContext AdminContext { get; } = new RequestContext
            {
                CurrentUser = new UserInfo { Id = 1, Role = UserRole.Admin }
            };

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
