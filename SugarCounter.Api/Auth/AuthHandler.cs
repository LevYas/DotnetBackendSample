using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using SugarCounter.Core.Sessions;
using SugarCounter.Core.Users;

namespace SugarCounter.Api.Auth
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthSchemeName = "SugarCounterAuth";
        public const string AuthenticationHeaderName = "Authentication";

        private readonly RequestContext _requestContext;
        private readonly ISessionsRepository _repository;

        public AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, RequestContext requestContext, ISessionsRepository repository)
            : base(options, logger, encoder, clock)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await _repository.ClearExpiredSessions();

            if (!Guid.TryParse(_requestContext.HttpContext.Request.Headers[AuthenticationHeaderName], out Guid AuthGuid))
                AuthGuid = Guid.Empty;

            UserInfo? user;
            if (AuthGuid == Guid.Empty || (user = await _repository.GetUserForSession(AuthGuid)) == null)
                return AuthenticateResult.Fail("Not authorized");

            _requestContext.CurrentUser = user;
            _requestContext.SessionId = AuthGuid;

            Claim[] Claims = new Claim[]
                {
                    new Claim (ClaimTypes.Name, _requestContext.CurrentUser.Login),
                    new Claim (ClaimTypes.NameIdentifier, _requestContext.CurrentUser.Id.ToString())
                };
            ClaimsIdentity Identity = new ClaimsIdentity(Claims, nameof(AuthHandler));
            ClaimsPrincipal Principal = new ClaimsPrincipal(Identity);

            return AuthenticateResult.Success(new AuthenticationTicket(Principal, AuthSchemeName));
        }
    }
}
