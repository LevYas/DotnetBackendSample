using System;
using System.Text;

namespace SugarCounter.DataAccess.Utils
{
    internal static class PasswordHasher
    {
        public const int HashedStringLength = 44; // 256 bits / 6 bits per char = 42.6666 char + padding

        public static string MakeSha256String(this string password)
        {
            byte[] hash;

            // It's better to use salted hash, but this is better than plain text :)
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-3.1
            using (var hasher = System.Security.Cryptography.SHA256.Create())
            {
                hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return Convert.ToBase64String(hash);
        }
    }
}
