using SugarCounter.Core.Shared;

namespace SugarCounter.Core.Users
{
    public class UsersRequest : PaginationParams
    {
        public UsersRequest() { }

        public UsersRequest(PaginationParams paginationParams, string? login = null)
        {
            PageNumber = paginationParams.PageNumber;
            ItemsPerPage = paginationParams.ItemsPerPage;
            Login = login;
        }

        public string? Login { get; set; }
    }
}
