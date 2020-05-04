using Microsoft.EntityFrameworkCore;
using SugarCounter.Core.Sessions;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Db;
using SugarCounter.DataAccess.Db.Entities;
using SugarCounter.DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class SessionsRepository : ISessionsRepository
    {
        private readonly AppDbContext _context;

        public SessionsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task ClearExpiredSessions()
        {
            TimeSpan expirationTime = TimeSpan.FromMinutes(30);
            DateTime lastValidDate = DateTime.Now - expirationTime;

            List<UserSession> expiredSessions = await _context.UserSessions
                .Where(us => us.LastAccessed <= lastValidDate).ToListAsync();

            if (expiredSessions.Count == 0)
                return;

            _context.UserSessions.RemoveRange(expiredSessions);

            await _context.SaveChangesAsync();
        }

        public async Task CreateSession(Guid sessionId, int userId)
        {
            UserSession? existingSession = await _context.UserSessions.SingleOrDefaultAsync(us => us.UserInfoId == userId);

            if (existingSession != null)
                _context.UserSessions.Remove(existingSession);

            _context.UserSessions.Add(new UserSession { Session = sessionId, UserInfoId = userId, LastAccessed = DateTime.Now });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSession(Guid sessionId)
        {
            UserSession existingSession = await _context.UserSessions.SingleOrDefaultAsync(us => us.Session == sessionId);

            if (existingSession == null)
                return;

            _context.UserSessions.Remove(existingSession);
            await _context.SaveChangesAsync();
        }

        public async Task<UserInfo?> GetUserForSession(Guid sessionId)
        {
            UserSession userSession = await _context.UserSessions
                .Include(s => s.UserInfo)
                .SingleOrDefaultAsync(us => us.Session == sessionId);

            if (userSession == null)
                return null;

            userSession.LastAccessed = DateTime.Now;
            await _context.SaveChangesAsync();

            return userSession.UserInfo;
        }

        public async Task<int?> TryAuthenticateUser(string login, string password)
        {
            string loginToUse = login.Normalize();

            UserInfo? user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Login == loginToUse && u.PasswordHash == password.MakeSha256String());

            return user?.Id;
        }
    }
}
