using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using Xunit;

namespace Integration.Api.Controllers.Users
{
    public class UsersControllerUpdateUser : UsersControllerTestsBase
    {
        [Fact]
        public async void DoesNotDemoteRoot()
        {
            var editsDto = new UserEditsDto { Role = UserRole.Supervisor };

            var result = await UsersController.UpdateUser(1, editsDto);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
