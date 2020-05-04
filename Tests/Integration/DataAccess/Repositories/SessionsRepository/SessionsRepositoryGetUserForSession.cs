using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.SessionsRepository
{
    public class SessionsRepositoryGetUserForSession : IntegrationTestsBase
    {
        [Fact]
        public async Task RetrievesUser()
        {
            Guid sessionId = Guid.NewGuid();
            var userToUse = DbContext.Users.First();
            DbContext.UserSessions.Add(new UserSession
            {
                Session = sessionId,
                UserInfoId = userToUse.Id,
                LastAccessed = DateTime.Now
            });
            await DbContext.SaveChangesAsync();

            UserInfo? user = await AuthRepo.GetUserForSession(sessionId);

            Assert.Equal(userToUse.Id, user!.Id);
            Assert.Equal(userToUse.Login, user.Login);
        }
    }
}
