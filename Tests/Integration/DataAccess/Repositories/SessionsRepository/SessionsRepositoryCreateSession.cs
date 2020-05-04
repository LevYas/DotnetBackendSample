using Microsoft.EntityFrameworkCore;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.SessionsRepository
{
    public class SessionsRepositoryCreateSession : IntegrationTestsBase
    {
        [Fact]
        public async Task CreatesSession()
        {
            Guid sessionId = Guid.NewGuid();
            UserInfo userToUse = DbContext.Users.First();

            await AuthRepo.CreateSession(sessionId, userToUse.Id);

            UserSession userSession = await DbContext.UserSessions.SingleOrDefaultAsync(us => us.Session == sessionId);
            Assert.Equal(userToUse.Id, userSession.UserInfo.Id);
        }

        [Fact]
        public async Task DoesNotCreateSessionDuplicates()
        {
            var userToUse = DbContext.Users.First();
            Guid sessionId1 = Guid.NewGuid();
            Guid sessionId2 = Guid.NewGuid();

            await AuthRepo.CreateSession(sessionId1, userToUse.Id);
            await AuthRepo.CreateSession(sessionId2, userToUse.Id);

            Assert.Single(DbContext.UserSessions.Where(s => s.UserInfoId == userToUse.Id));
        }
    }
}
