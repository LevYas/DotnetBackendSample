using System;

namespace SugarCounter.Api.Controllers.Auth.Dto
{
    public class LoginOutputDto
    {
        public static LoginOutputDto Failed => new LoginOutputDto { IsSuccessful = false };

        public bool IsSuccessful { get; set; }
        public Guid AuthGuid { get; set; }
    }
}
