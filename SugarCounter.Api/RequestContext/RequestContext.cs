using Microsoft.AspNetCore.Http;
using SugarCounter.Core.Users;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SugarCounter.Api
{
    public class RequestContext
    {
        private Guid? _sessionId;
        private HttpContext? _httpContext;

        public HttpContext HttpContext
        {
            get => ensureNotNull(_httpContext);
            set { _httpContext = value; }
        }

        public Guid SessionId
        {
            get => ensureNotNull(_sessionId).Value;
            set => _sessionId = value;
        }

        public UserInfo CurrentUser
        {
            get => ensureNotNull(CurrentUserRaw);
            set => CurrentUserRaw = value;
        }

        public UserInfo? CurrentUserRaw { get; private set; }

        [return: NotNull]
        private TField ensureNotNull<TField>(TField field, [CallerMemberName] string? propName = null)
        {
            return field
                ?? throw new NullReferenceException($"{propName} should not be null here," +
                        $"check AuthenticationHandler or {nameof(RequestContextCapturer)} middleware");
        }
    }
}
