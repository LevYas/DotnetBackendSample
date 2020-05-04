using Functional.ApiCallers;
using System.Net.Http;
using Xunit;

namespace Functional
{
    public class ApiTestsBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient Client;

        public ApiTestsBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();

            FoodApi = new FoodApiCaller(Client);
            UsersApi = new UsersApiCaller(Client);
            AuthApi = new AuthApiCaller(Client);
        }

        protected FoodApiCaller FoodApi { get; }
        protected UsersApiCaller UsersApi { get; }
        protected AuthApiCaller AuthApi { get; }
    }
}
