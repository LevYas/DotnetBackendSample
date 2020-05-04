using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Users
{
    public class SupervisorActionsTests : ApiTestsBase
    {
        private readonly UserInfoDto _supervisor;

        public SupervisorActionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
            AuthApi.LoginAsRoot().Wait();

            Task<UserInfoDto> newUserTask = UsersApi.CreateSupervisor();
            newUserTask.Wait();
            _supervisor = newUserTask.Result;

            AuthApi.Login(_supervisor.Login).Wait();
        }

        [Fact]
        public async Task CanGetThemselves()
        {
            UserInfoDto result = await UsersApi.GetCurrentUser();

            Assert.Equal(_supervisor.Login, result.Login);
        }

        [Fact]
        public async Task CanPromoteUserToManager()
        {
            UserInfoDto user = await UsersApi.CreateUser();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Supervisor };

            var response = await UsersApi.SendUpdateUser(user.Id, edits);

            response.EnsureSuccessStatusCode();
            UserInfoDto result = await UsersApi.GetUser(user.Id);
            Assert.Equal(UserRole.Supervisor, result.Role);
        }

        [Fact]
        public async Task CanDemoteUserFromManager()
        {
            UserInfoDto newUser = await UsersApi.CreateSupervisor();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.User };

            await UsersApi.UpdateUser(newUser.Id, edits);

            UserInfoDto result = await UsersApi.GetUser(newUser.Id);
            Assert.Equal(UserRole.User, result.Role);
        }

        [Fact]
        public async Task CanNotPromoteUserToAdmin()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Admin};

            HttpResponseMessage response = await UsersApi.SendUpdateUser(newUser.Id, edits);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CanGetOtherManager()
        {
            UserInfoDto newUser = await UsersApi.CreateSupervisor();

            UserInfoDto result = await UsersApi.GetUser(newUser.Id);

            Assert.Equal(newUser.Login, result.Login);
        }

        [Fact]
        public async Task CanNotGetAdmin()
        {
            await AuthApi.LoginAsRoot();
            UserInfoDto newUser = await UsersApi.CreateAdmin();
            await AuthApi.Login(_supervisor.Login);

            HttpResponseMessage response = await UsersApi.SendGetUser(newUser.Id);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanGetUsersListWithoutAdmins()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();

            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();

            Assert.DoesNotContain(result.Items, u => u.Role == UserRole.Admin);
        }

        [Fact]
        public async Task CanNotUpdateAdmin()
        {
            await AuthApi.LoginAsRoot();
            UserInfoDto newUser = await UsersApi.CreateAdmin();
            await AuthApi.Login(_supervisor.Login);
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Supervisor };

            HttpResponseMessage response = await UsersApi.SendUpdateUser(newUser.Id, edits);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanNotPromoteHimselfToAdmin()
        {
            UserEditsDto edits = new UserEditsDto { Role = UserRole.Admin };

            HttpResponseMessage response = await UsersApi.SendUpdateUser(_supervisor.Id, edits);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CanDeleteOtherUser()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();

            await UsersApi.DeleteUser(newUser.Id);

            HttpResponseMessage userByIdresponse = await UsersApi.SendGetUser(newUser.Id);
            Assert.Equal(HttpStatusCode.NotFound, userByIdresponse.StatusCode);

            PaginatedListDto<UserInfoDto> result = await UsersApi.GetUsers();
            Assert.DoesNotContain(result.Items, u => u.Login == newUser.Login);
        }

        [Fact]
        public async Task CanNotDeleteAdmin()
        {
            await AuthApi.LoginAsRoot();
            UserInfoDto newUser = await UsersApi.CreateAdmin();
            await AuthApi.Login(_supervisor.Login);

            HttpResponseMessage deleteResponse = await UsersApi.SendDeleteUser(newUser.Id);

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

            await AuthApi.LoginAsRoot();
            await UsersApi.GetUser(newUser.Id);
        }
    }
}
