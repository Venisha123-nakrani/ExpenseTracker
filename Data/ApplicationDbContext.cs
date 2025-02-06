using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<RecurringExpense> RecurringExpense { get; set; }
        public DbSet<ExpenseReport> ExpenseReport { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<Attachment> Attachment { get; set; }

    }
}
