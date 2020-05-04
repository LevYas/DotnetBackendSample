using SugarCounter.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SugarCounter.Api.Auth;
using Microsoft.AspNetCore.Authentication;
using SugarCounter.Api.Controllers.Food;

namespace SugarCounter.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDataAccess(Configuration.GetConnectionString("DefaultConnection"));
            services.AddControllers();
            services.AddApiVersioning();

            services.AddAuthentication(AuthHandler.AuthSchemeName)
                .AddScheme<AuthenticationSchemeOptions, AuthHandler>(AuthHandler.AuthSchemeName, _ => { });

            services.AddScoped(_ => new RequestContext());
            services.AddHttpClient<INutritionDataProvider, NutritionixClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error");

            app.UseRouting();

            app.UseMiddleware<RequestContextCapturer>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
