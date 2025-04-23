using ExpenseTracker.Data;
using ExpenseTracker.Model;

namespace ExpenseTracker.Utilities
{
    public class ActivityLogger
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogUserActivity(int userId, string action)
        {
            var activity = new UserActivity
            {
                UserID = userId,
                Action = action,
                Timestamp = DateTime.Now
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }
    }
}
