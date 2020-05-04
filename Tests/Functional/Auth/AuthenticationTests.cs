using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Auth
{
    public class AuthenticationTests : ApiTestsBase
    {
        public AuthenticationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task ReturnsUnauthorizedForAnonymous()
        {
            HttpResponseMessage allUsersResponse = await UsersApi.SendGetUsers();
            Assert.Equal(HttpStatusCode.Unauthorized, allUsersResponse.StatusCode);
        }

        [Fact]
        public async Task AcceptsRequestsAfterLogin()
        {
            await AuthApi.LoginAsRoot();

            HttpResponseMessage allUsersResponse = await UsersApi.SendGetUsers();
            Assert.Equal(HttpStatusCode.OK, allUsersResponse.StatusCode);
        }

        [Fact]
        public async Task ReturnsUnauthorizedAfterLogout()
        {
            await AuthApi.LoginAsRoot();

            await AuthApi.SendLogout();

            HttpResponseMessage allUsersResponse = await UsersApi.SendGetUsers();
            Assert.Equal(HttpStatusCode.Unauthorized, allUsersResponse.StatusCode);
        }
    }
}
