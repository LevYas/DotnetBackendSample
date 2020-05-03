using SugarCounter.Core.Users;
using System;

namespace SugarCounter.DataAccess.Db.Entities
{
    internal class UserSession
    {
        public int Id { get; set; }

        public Guid Session { get; set; }
        public int UserInfoId { get; set; }
        public DateTime LastAccessed { get; set; }

        public UserInfo UserInfo { get; set; } = null!;
    }
}
