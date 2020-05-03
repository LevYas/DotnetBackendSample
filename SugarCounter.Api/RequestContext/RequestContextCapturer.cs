using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SugarCounter.Api
{
    internal class RequestContextCapturer
    {
        private readonly RequestDelegate _next;

        public RequestContextCapturer(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext Context, RequestContext requestContext)
        {
            requestContext.HttpContext = Context;

            await _next(Context);
        }
    }
}
