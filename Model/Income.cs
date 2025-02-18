using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class Income
    {

        [Key]
        public int IncomeID { get; set; }
        public int UserID { get; set; }
        public int IncomeCategoryID { get; set; }
        public int PaymentModeID { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime IncomeDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public IncomeCategory IncomeCategory { get; set; }
        public Payment Payment { get; set; }

    }
}
