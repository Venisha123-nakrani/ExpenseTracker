using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Model;
using System;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RecurringExpense> RecurringExpenses { get; set; }
        public DbSet<ExpenseReport> ExpenseReports { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    FullName = "Admin User",
                    Email = "admin@expense.com",
                    PasswordHash = "Admin@123", // ✅ Normal password (only for testing!)
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0) // ✅ Static DateTime
                },
                new User
                {
                    UserID = 2,
                    FullName = "John Doe",
                    Email = "johndoe@example.com",
                    PasswordHash = "User@123", // ✅ Normal password (only for testing!)
                    CreatedAt = new DateTime(2024, 1, 2, 12, 0, 0) // ✅ Static DateTime
                }
            );

            modelBuilder.Entity<IncomeCategory>()
                .HasMany(ic => ic.Incomes)
                .WithOne(i => i.IncomeCategory)
                .HasForeignKey(i => i.IncomeCategoryID);

            modelBuilder.Entity<Income>()
                .HasOne(i => i.Payment)
                .WithMany(p => p.Incomes)
                .HasForeignKey(i => i.PaymentModeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpenseCategory>()
               .HasMany(ec => ec.Expenses)
               .WithOne(e => e.Category)
               .HasForeignKey(e => e.ExpenseCategoryID);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Payment)
                .WithMany(p => p.Expenses)
                .HasForeignKey(e => e.PaymentModeID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
