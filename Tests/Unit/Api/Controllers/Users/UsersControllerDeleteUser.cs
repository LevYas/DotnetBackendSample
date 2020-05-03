using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Core.Users;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Users
{
    public class UsersControllerDeleteUser
    {
        private readonly Mock<IUsersRepository> _repoMock;
        private readonly UsersController _controller;

        public UsersControllerDeleteUser()
        {
            _repoMock = new Mock<IUsersRepository>();
            var requestContext = new RequestContext
            {
                CurrentUser = new UserInfo { Role = UserRole.Admin }
            };
            _controller = new UsersController(requestContext, _repoMock.Object);
        }

        [Fact]
        public async Task ReturnsNoContentOnSuccess()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(new UserInfo());
            _repoMock.Setup(r => r.Delete(It.IsAny<UserInfo>())).ReturnsAsync(true);

            ActionResult actionResult = await _controller.DeleteUser(1);

            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task ReturnsNotFoundForNonExistingUser()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((UserInfo?)null);

            ActionResult actionResult = await _controller.DeleteUser(1);

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }
    }
}
