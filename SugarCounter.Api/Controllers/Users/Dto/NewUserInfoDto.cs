using SugarCounter.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace SugarCounter.Api.Controllers.Users.Dto
{
    public class NewUserInfoDto
    {
        [StringLength(UserInfoLimits.MaxLoginLength, MinimumLength = 1)]
        [RegularExpression(UserInfoLimits.LoginRegex, ErrorMessage = UserInfoLimits.LoginRegexErrorMessage)]
        public string Login { get; set; } = default!;

        [StringLength(UserInfoLimits.MaxPasswordLength, MinimumLength = 1)]
        public string Password { get; set; } = default!;

        public UserRole? Role { get; set; }
    }
}
