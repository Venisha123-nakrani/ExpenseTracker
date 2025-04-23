using BCrypt.Net;
using Microsoft.AspNetCore.Identity;

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


        private static readonly PasswordHasher<string> hasher = new();

        public static string PasswordHash(string password)
        {
            return hasher.HashPassword(null, password); // No user object needed
        }

        public static bool PasswordVerify(string enteredPassword, string storedHash)
        {
            var result = hasher.VerifyHashedPassword(null, storedHash, enteredPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

}
