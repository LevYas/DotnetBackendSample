using SugarCounter.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarCounter.Core.Sessions
{
    public interface ISessionsRepository
    {
        Task<int?> TryAuthenticateUser(string login, string password);
        Task CreateSession(Guid sessionId, int userInfoId);
        Task ClearExpiredSessions();
        Task<UserInfo?> GetUserForSession(Guid sessionId);
        Task DeleteSession(Guid sessionId);
    }
}
