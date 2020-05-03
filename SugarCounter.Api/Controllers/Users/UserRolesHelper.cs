using SugarCounter.Core.Users;

namespace SugarCounter.Api.Controllers.Users
{
    public static class UserRolesHelper
    {
        public static bool IsAssessForbidden(UserInfo? currentUser, UserInfo targetUser)
        {
            if (currentUser == null)
                return true;

            if (currentUser.Id == targetUser.Id)
                return false;

            return currentUser.Role switch
            {
                UserRole.User => true,
                UserRole.Admin => false,
                UserRole.Supervisor => targetUser.Role != UserRole.User && targetUser.Role != UserRole.Supervisor,
                _ => true, // How to improve: alert developers
            };
        }

        public static bool CanAssignRoleForNewUser(UserInfo? currentUser, UserRole desiredRole)
        {
            // for anonymous access allow to create only users
            if (currentUser == null || currentUser.Role == UserRole.User)
                return desiredRole == UserRole.User;

            return currentUser.Role switch
            {
                UserRole.Admin => true,
                UserRole.Supervisor => desiredRole == UserRole.User || desiredRole == UserRole.Supervisor,
                _ => false, // How to improve: alert developers
            };
        }

        public static bool CanUserBeDeleted(UserInfo targetUser) => targetUser.Login != "root";

        public static bool IsRoleChangeAllowed(UserInfo currentUser, UserInfo targetUser, UserRole to)
        {
            if (targetUser.Login == "root")
                return false;

            return currentUser.Role switch
            {
                UserRole.Admin => true,
                UserRole.User => false,
                UserRole.Supervisor => canSupervisorChangeRole(targetUser.Role, to),
                _ => false, // How to improve: alert developers
            };
        }

        private static bool canSupervisorChangeRole(UserRole from, UserRole to)
        {
            return (from, to) switch
            {
                (UserRole.Supervisor, UserRole.User) => true,
                (UserRole.User, UserRole.Supervisor) => true,
                _ => false,
            };
        }
    }
}
