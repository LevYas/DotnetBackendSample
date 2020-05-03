using SugarCounter.Api.Controllers.Users;
using SugarCounter.Core.Users;
using Xunit;

namespace Unit.Api.Controllers.Users
{
    public class UserRolesHelperCanAssignRoleForNewUser
    {
        [Theory]
        [InlineData(UserRole.User, true)]
        [InlineData(UserRole.Supervisor, false)]
        [InlineData(UserRole.Admin, false)]
        public void CanAssignOnlyUserForAnonymous(UserRole desiredRole, bool expected)
        {
            Assert.Equal(expected, UserRolesHelper.CanAssignRoleForNewUser(null, desiredRole));
        }

        [Theory]
        [InlineData(UserRole.User, UserRole.User, true)]
        [InlineData(UserRole.User, UserRole.Supervisor, false)]
        [InlineData(UserRole.User, UserRole.Admin, false)]
        [InlineData(UserRole.Supervisor, UserRole.User, true)]
        [InlineData(UserRole.Supervisor, UserRole.Supervisor, true)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, false)]
        [InlineData(UserRole.Admin, UserRole.User, true)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, true)]
        [InlineData(UserRole.Admin, UserRole.Admin, true)]
        public void CanAssignRoleForNewUserWorks(UserRole currentUserRole, UserRole desiredRole, bool expected)
        {
            UserInfo currentUser = new UserInfo { Role = currentUserRole };

            Assert.Equal(expected, UserRolesHelper.CanAssignRoleForNewUser(currentUser, desiredRole));
        }
    }
}
