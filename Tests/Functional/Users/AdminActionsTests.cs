using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Users
{
    public class AdminActionsTests : ApiTestsBase
    {
        public AdminActionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
            AuthApi.LoginAsRoot().Wait();
        }

        [Fact]
        public async Task CanPromoteToAdmin()
        {
            UserInfoDto user = await UsersApi.CreateUser();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Admin };

            var response = await UsersApi.SendUpdateUser(user.Id, edits);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CanGetFullUserList()
        {
            UserInfoDto newUser = await UsersApi.CreateAdmin();

            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();

            Assert.Contains(result.Items, u => u.Login == newUser.Login);
        }

        [Fact]
        public async Task CanGetThemselvesById()
        {
            UserInfoDto currentUser = await UsersApi.GetCurrentUser();

            UserInfoDto result = await UsersApi.GetUser(currentUser.Id);

            Assert.Equal(currentUser.Login, result.Login);
        }

        [Fact]
        public async Task CanGetUserById()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();

            UserInfoDto result = await UsersApi.GetUser(newUser.Id);

            Assert.Equal(newUser.Login, result.Login);
        }

        [Fact]
        public async Task CanGetOtherAdminById()
        {
            UserInfoDto newUser = await UsersApi.CreateAdmin();

            UserInfoDto result = await UsersApi.GetUser(newUser.Id);

            Assert.Equal(newUser.Login, result.Login);
        }

        [Fact]
        public async Task CanUpdateOtherAdmin()
        {
            UserInfoDto newUser = await UsersApi.CreateAdmin();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Supervisor };

            await UsersApi.UpdateUser(newUser.Id, edits);

            UserInfoDto result = await UsersApi.GetUser(newUser.Id);
            Assert.Equal(edits.Role, result.Role);
        }

        [Fact]
        public async Task CanDeleteOtherAdmin()
        {
            UserInfoDto newUser = await UsersApi.CreateAdmin();

            await UsersApi.DeleteUser(newUser.Id);

            HttpResponseMessage userByIdResponse = await UsersApi.SendGetUser(newUser.Id);
            Assert.Equal(HttpStatusCode.NotFound, userByIdResponse.StatusCode);

            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();
            Assert.DoesNotContain(result.Items, u => u.Login == newUser.Login);
        }
    }
}
