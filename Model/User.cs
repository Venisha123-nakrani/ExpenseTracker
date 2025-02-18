using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Expense> Expenses { get; set; }
        public ICollection<ExpenseReport> ExpenseReports { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<RecurringExpense> RecurringExpenses { get; set; }

        //public bool RememberMe { get; set; }

    }
}
