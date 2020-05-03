using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SugarCounter.Core.Users;

namespace SugarCounter.Api.Auth
{
    public class AuthorizeForAttribute : TypeFilterAttribute
    {
        public AuthorizeForAttribute(params UserRole[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly UserRole[] _claim;
        private readonly RequestContext _requestContext;

        public AuthorizeFilter(RequestContext requestContext, params UserRole[] claim)
        {
            _requestContext = requestContext;
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool isUserAuthorized = false;
            foreach (UserRole item in _claim)
            {
                if (_requestContext.CurrentUser.Role != item)
                    continue;

                isUserAuthorized = true;
                break;
            }

            if (!isUserAuthorized)
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}