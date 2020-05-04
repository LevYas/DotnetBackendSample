using Functional.ApiCallers;
using SugarCounter.Api.Controllers.Users.Dto;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Users
{
    public class UserCreationTests : ApiTestsBase
    {
        public UserCreationTests(CustomWebApplicationFactory factory) : base(factory) { }

        // This cannot be tested with in-memory database, since it executes queries sequentially
        [Fact]
        public async Task DoesNotCreateDuplicatesOnConcurrentUserCreation()
        {
            Task<HttpResponseMessage>[] creationTasks = new Task<HttpResponseMessage>[100];
            var newUser = new NewUserInfoDto { Login = "login" + DateTime.Now.Ticks, Password = ApiCaller.PasswordForUsers };

            for (int i = 0; i < 100; i++)
                creationTasks[i] = Client.PostAsJsonAsync(UsersApiCaller.UsersUrl, newUser);

            Task.WaitAll(creationTasks);

            await Assert.Single(creationTasks, t => t.Result.IsSuccessStatusCode);
            Assert.Equal(creationTasks.Length - 1, creationTasks.Count(t => t.Result.StatusCode == HttpStatusCode.Conflict));
        }

        [Theory]
        [InlineData("")]
        [InlineData("newLogin~")]
        [InlineData("log.")]
        [InlineData(".lo")]
        [InlineData(",-")]
        [InlineData("+1")]
        [InlineData("abcdefghijabcdefghijabcdefghijabcdefghijz")]
        public async Task ReportsErrorOnWrongLogin(string login)
        {
            var response = await UsersApi.SendCreateUser(new NewUserInfoDto { Login = login, Password = ApiCaller.PasswordForUsers });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("login")]
        [InlineData("5")]
        [InlineData("5mix5544")]
        [InlineData("abcdefghijabcdefghijabcdefghijabcdefghij")]
        public async Task AcceptsCorrectLogins(string login)
        {
            var response = await UsersApi.SendCreateUser(new NewUserInfoDto { Login = login, Password = ApiCaller.PasswordForUsers });

            response.EnsureSuccessStatusCode();
        }
    }
}
