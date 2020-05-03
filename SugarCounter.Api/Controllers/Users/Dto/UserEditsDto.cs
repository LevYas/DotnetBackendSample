using SugarCounter.Core.Users;

namespace SugarCounter.Api.Controllers.Users.Dto
{
    public class UserEditsDto
    {
        // How to improve: support login change
        public UserRole? Role { get; set; }
    }
}
