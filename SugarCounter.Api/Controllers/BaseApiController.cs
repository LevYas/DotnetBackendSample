using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Auth;
using SugarCounter.Core.Shared;
using System;

namespace SugarCounter.Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = AuthHandler.AuthSchemeName)]
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult<TResult> Match<TResult, TInput, TError>(Res<TInput, TError> res, Func<TInput, TResult> onOk,
                params (TError, Func<ActionResult<TResult>>)[] errHandlers)
            where TError : struct, Enum
        {
            return res.Match(
                onOk: val => onOk(val),
                defaultHandler: () => reportMissingHandler(typeof(TError)),
                errorHandlers: errHandlers
            );
        }

        protected ActionResult Match<TError>(Res<TError> res, Func<ActionResult> onOk,
                params (TError, Func<ActionResult>)[] errHandlers)
            where TError : struct, Enum
        {
            return res.Match(
                onOk: () => onOk(),
                defaultHandler: () => reportMissingHandler(typeof(TError)),
                errorHandlers: errHandlers
            );
        }

        private ActionResult reportMissingHandler(Type errorType)
        {
            return Problem($"To developers: not all the errors handled from {errorType.Name}");
        }
    }
}
