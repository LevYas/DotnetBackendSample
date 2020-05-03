using System;

namespace SugarCounter.Core.Users
{
    public class UserInfo
    {
        public int Id { get; set; }

        public UserRole Role { get; set; }

        public string Login { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }
    }
}
