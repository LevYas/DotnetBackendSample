using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.Auth.Dto;
using SugarCounter.Core.Sessions;
using System;
using System.Threading.Tasks;

namespace SugarCounter.Api.Controllers.Auth
{
    public class AuthController : BaseApiController
    {
        private readonly RequestContext _requestContext;
        private readonly ISessionsRepository _sessionsRepo;

        public AuthController(RequestContext requestContext, ISessionsRepository sessionsRepo)
        {
            _requestContext = requestContext;
            _sessionsRepo = sessionsRepo;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginOutputDto>> Login(LoginInputDto input)
        {
            int? userId = await _sessionsRepo.TryAuthenticateUser(input.Login, input.Password);

            if (userId == null)
                return LoginOutputDto.Failed;

            Guid newSessionId = Guid.NewGuid();

            await _sessionsRepo.CreateSession(newSessionId, userId.Value);

            return new LoginOutputDto
            {
                IsSuccessful = true,
                AuthGuid = newSessionId
            };
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _sessionsRepo.DeleteSession(_requestContext.SessionId);
            return NoContent();
        }
    }
}
