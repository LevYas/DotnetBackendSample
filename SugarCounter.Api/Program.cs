using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SugarCounter.DataAccess.Db;

namespace SugarCounter.Api
{
    public class Program
    {
        // Useful CLI commands:
        //  Production env:
        //      dotnet watch --project SugarCounter.Api/SugarCounter.Api.csproj run --configuration Release
        //  Development env:
        //      dotnet watch --project SugarCounter.Api/SugarCounter.Api.csproj run --launch-profile SugarCounter.Api.Dev
        // Be sure that ./Properties/launchSettings.json contains properly configured profiles!
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                try
                {
                    DbInitializer.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                    return;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
