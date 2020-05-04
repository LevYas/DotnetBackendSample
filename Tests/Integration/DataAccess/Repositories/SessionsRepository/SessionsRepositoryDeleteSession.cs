using SugarCounter.Core.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.SessionsRepository
{
    public class SessionsRepositoryDeleteSession : IntegrationTestsBase
    {
        [Fact]
        public async Task DeletesSession()
        {
            Guid sessionId = Guid.NewGuid();
            UserInfo userToUse = DbContext.Users.First();
            await AuthRepo.CreateSession(sessionId, userToUse.Id);

            await AuthRepo.DeleteSession(sessionId);

            Assert.Empty(DbContext.UserSessions.Where(us => us.Session == sessionId));
        }
    }
}
