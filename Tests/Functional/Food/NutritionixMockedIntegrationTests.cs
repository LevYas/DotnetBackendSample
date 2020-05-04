using SugarCounter.Api.Controllers.Food.Dto;
using System;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Functional.Food
{
    public class NutritionixMockedIntegrationTests : ApiTestsBase, IDisposable
    {
        private readonly WireMockServer _server;

        public NutritionixMockedIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
            _server = WireMockServer.Start(9999);

            _server
                .Given(Request.Create()
                    .WithPath("/v2/natural/nutrients")
                    .WithBody(@"{""query"":""2 apples""}")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(@"{""foods"":[{""food_name"":""apples"",""nf_sugars"":199}]}"));

            AuthApi.LoginAsRoot().Wait();
        }

        [Fact]
        public async Task WhenSugarIsNotSetApiGetsItFromNutritionix()
        {
            FoodItemInputDto newItem = FoodApi.CreateNewFoodInput("2 apples");
            newItem.SugarMass = null;

            FoodItemDto result = await FoodApi.AddItemForCurrentUser(newItem);

            Assert.Equal(199, result.SugarMass);
        }

        public void Dispose()
        {
            _server.Stop();
        }
    }
}
