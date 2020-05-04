using SugarCounter.Core.Users;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.SessionsRepository
{
    public class SessionsRepositoryTryAuthenticateUser : IntegrationTestsBase
    {
        [Fact]
        public async Task AuthenticatesUser()
        {
            UserInfo userToUse = DbContext.Users.First();

            int? userId = await AuthRepo.TryAuthenticateUser(userToUse.Login, "Admin");

            Assert.NotNull(userId);
            Assert.Equal(userId, userToUse.Id);
        }

        [Fact]
        public async Task DoesNotAuthenticateWithWrongPassword()
        {
            UserInfo userToUse = DbContext.Users.First();

            int? userId = await AuthRepo.TryAuthenticateUser(userToUse.Login, "Admi");

            Assert.Null(userId);
        }
    }
}
