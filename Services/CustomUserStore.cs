using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Model;

namespace ExpenseTracker.Data
{
    public class CustomUserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private readonly ApplicationDbContext _context;

        public CustomUserStore(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(task =>
            {
                return task.IsCompletedSuccessfully
                    ? IdentityResult.Success
                    : IdentityResult.Failed(new IdentityError { Description = "Error creating user" });
            });
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(task =>
            {
                return task.IsCompletedSuccessfully
                    ? IdentityResult.Success
                    : IdentityResult.Failed(new IdentityError { Description = "Error deleting user" });
            });
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _context.Users.FindAsync(new object[] { int.Parse(userId) }, cancellationToken).Result;
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Email.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToUpper());
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(task =>
            {
                return task.IsCompletedSuccessfully
                    ? IdentityResult.Success
                    : IdentityResult.Failed(new IdentityError { Description = "Error updating user" });
            });
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            // Assuming your User model uses an integer as UserID and you're using it as the unique identifier
            return Task.FromResult(user.UserID.ToString());
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
