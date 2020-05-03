using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api;
using SugarCounter.Api.Controllers.Users;
using SugarCounter.Api.Controllers.Users.Dto;
using SugarCounter.Core.Users;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Users
{
    public class UsersControllerUpdateUser
    {
        private readonly Mock<IUsersRepository> _repoMock;
        private readonly UsersController _controller;
        private readonly UserInfo _currentUser;

        public UsersControllerUpdateUser()
        {
            _currentUser = new UserInfo { Id = 1, Role = UserRole.Admin };

            _repoMock = new Mock<IUsersRepository>();
            _repoMock.Setup(r => r.Update(It.IsAny<UserInfo>())).ReturnsAsync(true);

            var requestContext = new RequestContext { CurrentUser = _currentUser };
            _controller = new UsersController(requestContext, _repoMock.Object);
        }

        [Fact]
        public async Task ReturnsNoContentOnSuccess()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(new UserInfo { Login = "l" });

            var actionResult = await _controller.UpdateUser(1, new UserEditsDto());

            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task ReturnsNotFoundForNonExistingUser()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((UserInfo?)null);

            var actionResult = await _controller.UpdateUser(1, new UserEditsDto());

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async void UpdatesRole()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(_currentUser);
            var edits = new UserEditsDto { Role = UserRole.Supervisor };

            await _controller.UpdateUser(_currentUser.Id, edits);

            _repoMock.Verify(r => r.Update(It.Is<UserInfo>(u => u.Role == edits.Role)));
        }
    }
}
