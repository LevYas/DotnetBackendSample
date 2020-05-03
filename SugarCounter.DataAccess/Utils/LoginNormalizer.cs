namespace SugarCounter.DataAccess.Utils
{
    internal static class LoginNormalizer
    {
        public static string NormalizeLogin(this string login)
        {
            return login.Trim().ToLower();
        }
    }
}
