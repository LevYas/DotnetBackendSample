using SugarCounter.Api.Auth;
using SugarCounter.Api.Controllers.Auth.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace Functional.ApiCallers
{
    public class AuthApiCaller : ApiCaller
    {
        private const string _authUrl = "/api/v1/auth";

        public AuthApiCaller(HttpClient client) : base(client) { }

        public Task<LoginOutputDto> LoginAsRoot() => Login("root", "Admin");

        public async Task<LoginOutputDto> Login(string login, string? password = null)
        {
            var creds = new LoginInputDto { Login = login, Password = password ?? PasswordForUsers };
            LoginOutputDto loginResult = await ReadResponse<LoginOutputDto>(Client.PostAsJsonAsync($"{_authUrl}/login", creds));
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add(AuthHandler.AuthenticationHeaderName, loginResult.AuthGuid.ToString());
            return loginResult;
        }

        public async Task<HttpResponseMessage> SendLogout()
        {
            return await Client.PostAsync($"{_authUrl}/logout", new StringContent(""));
        }
    }
}
