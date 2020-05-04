using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using Xunit;

namespace Integration.Api.Controllers.Users
{
    public class UsersControllerDeleteUser : UsersControllerTestsBase
    {
        [Fact]
        public async void DeletesExistingUserFromDB()
        {
            ActionResult<UserInfoDto> res = await UsersController.CreateUser(CreateNewUserDto("login"));

            await UsersController.DeleteUser(res.Value.Id);

            PaginatedList<UserInfo>? result = await UsersRepo.GetList(false, DefaultRequestParams);
            Assert.DoesNotContain(result!.Items, u => u.Id == res.Value.Id);
        }

        [Fact]
        public async void DoesNotDeleteRoot()
        {
            var result = await UsersController.DeleteUser(1);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
