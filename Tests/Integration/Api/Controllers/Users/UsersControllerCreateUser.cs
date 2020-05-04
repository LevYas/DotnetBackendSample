using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using Xunit;

namespace Integration.Api.Controllers.Users
{
    public class UsersControllerCreateUser : UsersControllerTestsBase
    {
        [Fact]
        public async void CreateUserSavesUserToDb()
        {
            const string login = "login";

            await UsersController.CreateUser(CreateNewUserDto(login));

            PaginatedList<UserInfo>? result = await UsersRepo.GetList(false, DefaultRequestParams);
            Assert.Contains(result!.Items, u => u.Login == login);
        }

        [Fact]
        public async void CreatesUserWithUserRole()
        {
            var result = await UsersController.CreateUser(CreateNewUserDto("test1"));

            Assert.Equal(UserRole.User, result.Value.Role);
        }

        [Fact]
        public async void CreatesUserWithSpecifiedInfo()
        {
            const string login = "newlogin";

            var result = await UsersController.CreateUser(CreateNewUserDto(login));

            UserInfoDto userInfo = result.Value;
            Assert.Equal(login, userInfo.Login);
        }

        [Fact]
        public async void DoesNotCreateDuplicates()
        {
            const string login = "newLogin";
            await UsersController.CreateUser(CreateNewUserDto(login));

            var result = await UsersController.CreateUser(CreateNewUserDto(login));

            Assert.IsType<ConflictObjectResult>(result.Result);
        }
    }
}
