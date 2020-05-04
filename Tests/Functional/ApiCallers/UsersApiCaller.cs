using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Functional.ApiCallers
{
    public class UsersApiCaller : ApiCaller
    {
        public const string UsersUrl = "/api/v1/users";

        public UsersApiCaller(HttpClient client) : base(client) { }

        public Task<PaginatedListDto<UserInfoDto>> GetUsers(int? pageNumber = null, int? itemsPerPage = null)
        {
            return ReadResponse<PaginatedListDto<UserInfoDto>>(SendGetUsers(pageNumber, itemsPerPage));
        }

        public async Task<HttpResponseMessage> SendGetUsers(int? pageNumber = null, int? itemsPerPage = null)
        {
            return await Client.GetAsync(CombineUrl(UsersUrl, pageNumber, itemsPerPage));
        }

        public Task<UserInfoDto> GetCurrentUser() => ReadResponse<UserInfoDto>(SendGetCurrentUser());
        public async Task<HttpResponseMessage> SendGetCurrentUser() => await Client.GetAsync($"{UsersUrl}/me");

        public Task<UserInfoDto> GetUser(int id) => ReadResponse<UserInfoDto>(SendGetUser(id));
        public async Task<HttpResponseMessage> SendGetUser(int id) => await Client.GetAsync($"{UsersUrl}/{id}");

        public Task DeleteUser(int id) => EnsureSuccess(SendDeleteUser(id));
        public async Task<HttpResponseMessage> SendDeleteUser(int id) => await Client.DeleteAsync($"{UsersUrl}/{id}");

        public Task UpdateUser(int id, UserEditsDto dto) => EnsureSuccess(SendUpdateUser(id, dto));

        public async Task<HttpResponseMessage> SendUpdateUser(int id, UserEditsDto dto) =>
            await Client.PutAsJsonAsync($"{UsersUrl}/{id}", dto);

        public async Task<UserInfoDto> CreateUser(string loginBase = "login", UserRole role = UserRole.User)
        {
            var newUser = new NewUserInfoDto
            {
                Login = loginBase + DateTime.Now.Ticks,
                Password = PasswordForUsers,
                Role = role
            };
            return await ReadResponse<UserInfoDto>(SendCreateUser(newUser));
        }

        public async Task<HttpResponseMessage> SendCreateUser(NewUserInfoDto newUser) =>
            await Client.PostAsJsonAsync(UsersUrl, newUser);

        public Task<UserInfoDto> CreateSupervisor() => CreateUser("supervisor", UserRole.Supervisor);
        public Task<UserInfoDto> CreateAdmin() => CreateUser("admin", UserRole.Admin);
    }
}
