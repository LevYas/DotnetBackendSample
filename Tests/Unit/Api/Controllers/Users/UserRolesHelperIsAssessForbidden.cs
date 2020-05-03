using SugarCounter.Api.Controllers.Users;
using SugarCounter.Core.Users;
using Xunit;

namespace Unit.Api.Controllers.Users
{
    public class UserRolesHelperIsAssessForbidden
    {
        [Theory]
        [InlineData(UserRole.User, UserRole.User, true)]
        [InlineData(UserRole.User, UserRole.Supervisor, true)]
        [InlineData(UserRole.User, UserRole.Admin, true)]
        [InlineData(UserRole.Supervisor, UserRole.User, false)]
        [InlineData(UserRole.Supervisor, UserRole.Supervisor, false)]
        [InlineData(UserRole.Supervisor, UserRole.Admin, true)]
        [InlineData(UserRole.Admin, UserRole.User, false)]
        [InlineData(UserRole.Admin, UserRole.Supervisor, false)]
        [InlineData(UserRole.Admin, UserRole.Admin, false)]
        public void IsAssessForbiddenWorksForDifferentUsers(UserRole currentUserRole, UserRole targetUserRole, bool expected)
        {
            UserInfo currentUser = new UserInfo { Id = 3, Role = currentUserRole };
            UserInfo targetUser = new UserInfo { Id = 4, Role = targetUserRole };

            Assert.Equal(expected, UserRolesHelper.IsAssessForbidden(currentUser, targetUser));
        }

        [Theory]
        [InlineData(UserRole.User)]
        [InlineData(UserRole.Supervisor)]
        [InlineData(UserRole.Admin)]
        public void AllowesAccessForTheOwner(UserRole currentUserRole)
        {
            UserInfo currentUser = new UserInfo { Id = 3, Role = currentUserRole };
            UserInfo targetUser = new UserInfo { Id = 3, Role = currentUserRole };

            Assert.False(UserRolesHelper.IsAssessForbidden(currentUser, targetUser));
        }

        [Theory]
        [InlineData(UserRole.User)]
        [InlineData(UserRole.Supervisor)]
        [InlineData(UserRole.Admin)]
        public void ForbidsAccessForAnonymous(UserRole targetUserRole)
        {
            UserInfo targetUser = new UserInfo { Id = 3, Role = targetUserRole };

            Assert.True(UserRolesHelper.IsAssessForbidden(null, targetUser));
        }
    }
}
