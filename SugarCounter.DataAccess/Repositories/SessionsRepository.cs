using SugarCounter.Core.Sessions;
using SugarCounter.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class SessionsRepository : ISessionsRepository
    {
        public Task ClearExpiredSessions()
        {
            throw new NotImplementedException();
        }

        public Task CreateSession(Guid sessionId, int userInfoId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo?> GetUserForSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<int?> TryAuthenticateUser(string login, string password)
        {
            throw new NotImplementedException();
        }
    }
}
