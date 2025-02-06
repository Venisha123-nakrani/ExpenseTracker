using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        [Key]
        public int BudgetID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public ExpenseCategory Category { get; set; }
    }
}
