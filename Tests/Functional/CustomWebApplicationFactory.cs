using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SugarCounter.Api;
using SugarCounter.DataAccess.Db;
using System;

namespace Functional
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("functionalTesting.json")
                    .Build();

                config.AddConfiguration(integrationConfig);
            });

            builder.ConfigureServices(services =>
            {
                ServiceProvider sp = services.BuildServiceProvider();

                using IServiceScope scope = sp.CreateScope();
                IServiceProvider scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                try
                {
                    db.Database.EnsureDeleted();
                    DbInitializer.Initialize(db);
                }
                catch (Exception ex)
                {
                    string connectionString = scopedServices.GetRequiredService<IConfiguration>()
                        .GetConnectionString("DefaultConnection");

                    logger.LogError(ex, $"Failed to initialize the test database using connection string: '{connectionString}'");
                }
            });
        }
    }
}
