using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using System.Threading.Tasks;
using Xunit;

namespace Integration.Api.Controllers.Users
{
    public class UsersControllerGetUsers : UsersControllerTestsBase
    {
        [Fact]
        public async void ReturnsNewlyAddedUser()
        {
            ActionResult<UserInfoDto> result = await UsersController.CreateUser(CreateNewUserDto("login2"));

            ActionResult<PaginatedListDto<UserInfoDto>> usersResult = await UsersController.GetUsers();

            Assert.Contains(usersResult.Value.Items, user => user.Login == "login2");
        }

        [Fact]
        public async Task ProvidesCorrectPaginationInfo()
        {
            await prepareItems(15);

            var result = await UsersController.GetUsers(new UsersRequestDto(1, 10));

            PaginatedListDto<UserInfoDto> list = result.Value;
            Assert.Equal(10, list.ItemsPerPage);
            Assert.Equal(1, list.PageNumber);
            Assert.Equal(16, list.TotalCount); // root is +1 user
            Assert.Equal(2, list.TotalPages);
        }

        private async Task prepareItems(int count)
        {
            for (int i = 0; i < count; i++)
                await UsersController.CreateUser(CreateNewUserDto("login2" + i));
        }
    }
}
