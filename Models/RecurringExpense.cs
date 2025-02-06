using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class RecurringExpense
    {
        [Key]
        public int RecurringExpenseID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public ExpenseCategory Category { get; set; }
    }
}
