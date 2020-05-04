using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db;
using SugarCounter.DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(AppDbContext context, ILogger<UsersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Res<UserInfo, CreateUserError>> Create(string login, string password, UserRole role)
        {
            string loginToUse = login.Normalize();

            using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                bool doesUserExist = await _context.Users.AsNoTracking().AnyAsync(u => u.Login == loginToUse);

                if (doesUserExist)
                    return CreateUserError.UserAlreadyExists;

                var regDate = DateTime.Now; // assume clients and the server have one time zone
                var user = new UserInfo
                {
                    Login = loginToUse,
                    PasswordHash = password.MakeSha256String(),
                    Role = role,
                    RegistrationDate = regDate
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                transaction.Commit();
                _context.Entry(user).State = EntityState.Detached;

                return user;
            }
            catch (Exception ex)
            {
                if (ex?.InnerException is DbUpdateException)
                {
                    _logger.LogWarning(ex, "Failed to create new user, maybe because it's already exist");
                    return CreateUserError.UserAlreadyExists;
                }

                _logger.LogError(ex, "Failed to create new user");
                return CreateUserError.Unknown;
            }
        }

        public async Task<bool> Delete(UserInfo user)
        {
            _context.Users.Remove(user); // another option - just mark as deleted

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user");
                return false;
            }
        }

        public async Task<UserInfo?> GetById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<PaginatedList<UserInfo>?> GetList(bool excudeAdmins, UsersRequest request)
        {
            IQueryable<UserInfo> collection = _context.Users.AsNoTracking();

            if (excudeAdmins)
                collection = collection.Where(u => u.Role != UserRole.Admin);

            if (request.Login != null)
                collection = collection.Where(u => u.Login.Contains(request.Login, StringComparison.InvariantCultureIgnoreCase));

            try
            {
                int count = await collection.CountAsync();
                List<UserInfo> items = await collection.Paginate(request).ToListAsync();

                return new PaginatedList<UserInfo>(items, count, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve list of users");
                return null;
            }
        }

        public async Task<bool> Update(UserInfo userInfo)
        {
            _context.Entry(userInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
