namespace Development.API.Data
{
    public class ApplicationDataSeed
    {
        // Roles
        public const string AdminRole = "Administrator";
        public const string AuthorRole = "Author";

        // Default Users
        public const string AdminUserName = "admin@gmail.com";
        public const string AuthorUserName = "author@gmail.com";

        // Login Attempts
        public const int MaximumLoginAttempts = 3;
    }
}
