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
        private readonly IUsersRepository _repository;
        private readonly UsersController _controller;

        public UsersControllerDeleteUser()
        {
            _repository = GetRepository();
            _controller = new UsersController(GetContextForAdmin(), _repository);
        }

        [Fact]
        public async void DeletesExistingUserFromDB()
        {
            ActionResult<UserInfoDto> res = await _controller.CreateUser(CreateNewUserDto("login"));

            await _controller.DeleteUser(res.Value.Id);

            PaginatedList<UserInfo>? result = await _repository.GetList(false, DefaultRequestParams);
            Assert.DoesNotContain(result!.Items, u => u.Id == res.Value.Id);
        }

        [Fact]
        public async void DoesNotDeleteRoot()
        {
            var result = await _controller.DeleteUser(1);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
