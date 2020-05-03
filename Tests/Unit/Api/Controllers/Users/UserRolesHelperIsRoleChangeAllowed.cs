using SugarCounter.Api.Controllers.Users;
using SugarCounter.Core.Users;
using Xunit;

namespace Unit.Api.Controllers.Users
{
    public class UserRolesHelperIsRoleChangeAllowed
    {
        [Fact]
        public void ReturnsFalseForRoot()
        {
            UserInfo user = new UserInfo { Role = UserRole.Admin };
            UserInfo root = new UserInfo { Login = "root", Role = UserRole.Admin };

            Assert.False(UserRolesHelper.IsRoleChangeAllowed(user, root, UserRole.Supervisor));
        }

        [Theory]
        [InlineData(UserRole.User, UserRole.Supervisor, false)]
        [InlineData(UserRole.User, UserRole.Admin, false)]
        [InlineData(UserRole.Supervisor, UserRole.User, false)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, false)]
        [InlineData(UserRole.Admin, UserRole.User, false)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, false)]
        public void IsRoleChangeAllowedWorksForUser(UserRole from, UserRole to, bool expected)
        {
            UserInfo user = new UserInfo { Role = UserRole.User };

            Assert.Equal(expected, UserRolesHelper.IsRoleChangeAllowed(user, new UserInfo { Role = from }, to));
        }

        [Theory]
        [InlineData(UserRole.User, UserRole.Supervisor, true)]
        [InlineData(UserRole.Supervisor, UserRole.User, true)]
        [InlineData(UserRole.User, UserRole.Admin, false)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, false)]
        [InlineData(UserRole.Admin, UserRole.User, false)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, false)]
        public void IsRoleChangeAllowedWorksForSupervisor(UserRole from, UserRole to, bool expected)
        {
            UserInfo user = new UserInfo { Role = UserRole.Supervisor };

            Assert.Equal(expected, UserRolesHelper.IsRoleChangeAllowed(user, new UserInfo { Role = from }, to));
        }

        [Theory]
        [InlineData(UserRole.User, UserRole.Supervisor, true)]
        [InlineData(UserRole.User, UserRole.Admin, true)]
        [InlineData(UserRole.Supervisor, UserRole.User, true)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, true)]
        [InlineData(UserRole.Admin, UserRole.User, true)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, true)]
        public void IsRoleChangeAllowedWorksForAdmin(UserRole from, UserRole to, bool expected)
        {
            UserInfo user = new UserInfo { Role = UserRole.Admin };

            Assert.Equal(expected, UserRolesHelper.IsRoleChangeAllowed(user, new UserInfo { Role = from }, to));
        }
    }
}
