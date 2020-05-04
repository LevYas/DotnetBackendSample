using SugarCounter.DataAccess.Db.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.SessionsRepository
{
    public class SessionsRepositoryClearExpiredSessions : IntegrationTestsBase
    {
        [Fact]
        public async Task ClearsExpired()
        {
            DateTime now = DateTime.Now;
            DateTime lastAccessed = now - TimeSpan.FromMinutes(31);
            DbContext.UserSessions.Add(new UserSession { Session = Guid.NewGuid(), UserInfoId = 1, LastAccessed = lastAccessed });
            await DbContext.SaveChangesAsync();

            await AuthRepo.ClearExpiredSessions();

            Assert.Empty(DbContext.UserSessions);
        }
    }
}
