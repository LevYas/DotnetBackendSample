using SugarCounter.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace SugarCounter.Api.Controllers.Auth.Dto
{
    public class LoginInputDto
    {
        [StringLength(UserInfoLimits.MaxLoginLength, MinimumLength = 1)]
        public string Login { get; set; } = default!;

        [StringLength(UserInfoLimits.MaxPasswordLength, MinimumLength = 1)]
        public string Password { get; set; } = default!;
    }
}
