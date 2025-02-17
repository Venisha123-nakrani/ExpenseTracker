using BCrypt.Net;

namespace ExpenseTracker.Utilities
{
    public class PasswordHasher
    {
        // Hash a password (for user registration)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify a password (for user login)
        public static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
