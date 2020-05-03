using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace SugarCounter.Api.Controllers.Users.Dto
{
    public class UsersRequestDto : PaginationParamsDto
    {
        public UsersRequestDto() { }

        public UsersRequestDto(int pageNumber, int itemsPerPage)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
        }

        [StringLength(UserInfoLimits.MaxLoginLength, MinimumLength = 1)]
        [RegularExpression(UserInfoLimits.LoginRegex, ErrorMessage = UserInfoLimits.LoginRegexErrorMessage)]
        public string? Login { get; set; }

        public new UsersRequest ToRefined()
            => new UsersRequest(base.ToRefined(), Login);
    }
}
