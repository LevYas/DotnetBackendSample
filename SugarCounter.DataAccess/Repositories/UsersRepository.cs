using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class UsersRepository : IUsersRepository
    {
        public Task<Res<UserInfo, CreateUserError>> Create(string login, string password, UserRole role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo?> GetById(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<UserInfo>?> GetList(bool excudeAdmins, UsersRequest requestParams)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
