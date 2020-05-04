using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Users
{
    public class UserActionsTests : ApiTestsBase
    {
        private readonly UserInfoDto _user;

        public UserActionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
            Task<UserInfoDto> newUserTask = UsersApi.CreateUser();
            newUserTask.Wait();
            _user = newUserTask.Result;

            AuthApi.Login(_user.Login).Wait();
        }

        [Fact]
        public async Task CanGetThemselves()
        {
            UserInfoDto result = await UsersApi.GetCurrentUser();

            Assert.Equal(_user.Login, result.Login);
        }

        [Fact]
        public async Task CanNotGetOtherUser()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();

            HttpResponseMessage response = await UsersApi.SendGetUser(newUser.Id);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanNotKnowWhetherOtherUserExists()
        {
            HttpResponseMessage response = await UsersApi.SendGetUser(999);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanNotGetUsersList()
        {
            HttpResponseMessage response = await UsersApi.SendGetUsers();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CanNotUpdateOtherUser()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Supervisor };

            HttpResponseMessage response = await UsersApi.SendUpdateUser(newUser.Id, edits);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanNotPromoteHimself()
        {
            var edits = new UserEditsDto { Role = UserRole.Supervisor };

            HttpResponseMessage response = await UsersApi.SendUpdateUser(_user.Id, edits);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CanDeleteThemselves()
        {
            await UsersApi.DeleteUser(_user.Id);

            await AuthApi.LoginAsRoot();
            HttpResponseMessage userByIdresponse = await UsersApi.SendGetUser(_user.Id);
            Assert.Equal(HttpStatusCode.NotFound, userByIdresponse.StatusCode);

            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();
            Assert.DoesNotContain(result.Items, u => u.Login == _user.Login);
        }

        [Fact]
        public async Task CanNotDeleteOtherUser()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();

            HttpResponseMessage deleteResponse = await UsersApi.SendDeleteUser(newUser.Id);

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

            await AuthApi.LoginAsRoot();
            await UsersApi.GetUser(newUser.Id);
            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();
            Assert.Contains(result.Items, u => u.Login == newUser.Login);
        }
    }
}
