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
    public class UsersControllerCreateUser
    {
        private readonly Mock<IUsersRepository> _repoMock;
        private readonly RequestContext _requestContext;
        private readonly UsersController _controller;

        public UsersControllerCreateUser()
        {
            _repoMock = new Mock<IUsersRepository>();
            _repoMock
                .Setup(r => r.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .ReturnsAsync(new UserInfo());

            _requestContext = new RequestContext();
            _controller = new UsersController(_requestContext, _repoMock.Object);
        }

        [Theory]
        [InlineData(null, UserRole.User, true)]
        [InlineData(UserRole.User, UserRole.User, true)]
        [InlineData(UserRole.User, UserRole.Supervisor, false)]
        [InlineData(UserRole.User, UserRole.Admin, false)]
        [InlineData(UserRole.Supervisor, UserRole.User, true)]
        [InlineData(UserRole.Supervisor, UserRole.Supervisor, true)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, false)]
        [InlineData(UserRole.Admin, UserRole.User, true)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, true)]
        [InlineData(UserRole.Admin, UserRole.Admin, true)]
        public async Task RespectsRoleRules(UserRole? currentUserRole, UserRole newUserRole, bool shouldCreate)
        {
            if (currentUserRole != null)
                _requestContext.CurrentUser = new UserInfo { Role = currentUserRole.Value };
            NewUserInfoDto newUser = new NewUserInfoDto { Login = "login", Password = "123" };
            newUser.Role = newUserRole;

            var actionResult = await _controller.CreateUser(newUser);

            if (shouldCreate)
                Assert.Null(actionResult.Result);
            else
                Assert.IsType<ForbidResult>(actionResult.Result);
        }
    }
}
