using SugarCounter.Core.Shared;
using System.Threading.Tasks;

namespace SugarCounter.Core.Users
{
    public interface IUsersRepository
    {
        Task<Res<UserInfo, CreateUserError>> Create(string login, string password, UserRole role);
        Task<PaginatedList<UserInfo>?> GetList(bool excudeAdmins, UsersRequest requestParams);
        Task<UserInfo?> GetById(int userId);
        Task<bool> Update(UserInfo userInfo);
        Task<bool> Delete(UserInfo user);
    }

    public enum CreateUserError { UserAlreadyExists, Unknown }
}
