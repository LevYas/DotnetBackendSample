namespace SugarCounter.Core.Users
{
    public static class UserInfoLimits
    {
        public const int MaxLoginLength = 40;
        public const string LoginRegex = @"^[a-zA-Z\d]+$";
        public const string LoginRegexErrorMessage = "Login must consist of only letters and numbers";
        public const int MaxPasswordLength = 100;
    }
}
