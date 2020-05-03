using SugarCounter.Core.Users;
using System;

namespace SugarCounter.Api.Controllers.Users.Dto
{
    public class UserInfoDto
    {
        [Obsolete("For serializer only")]
        public UserInfoDto() => Login = "";

        public UserInfoDto(UserInfo model)
        {
            Id = model.Id;
            Login = model.Login;
            Role = model.Role;
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public UserRole Role { get; set; }
    }
}
