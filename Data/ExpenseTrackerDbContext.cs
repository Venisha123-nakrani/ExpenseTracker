using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ExpenseTrackerDbContext : IdentityDbContext<IdentityUser>
    {
        public ExpenseTrackerDbContext() { }
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options)
        {
        }
        public DbSet<Expense> Expense { get; set; } 
        public DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<ExpenseReport> ExpenseReport { get; set; }
        public DbSet<RecurringExpense> RecurringExpense { get; set; }
        public DbSet<Income> Income { get; set; }
        public DbSet<IncomeCategory> IncomeCategory { get; set; }
        public DbSet<Payment> Payment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseCategory>()
                .HasMany(ec => ec.Expenses)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryID);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Payment)
                .WithMany(p => p.Expenses)
                .HasForeignKey(e => e.PaymentModeID)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
