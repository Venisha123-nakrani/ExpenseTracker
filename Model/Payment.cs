using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class Payment
    {
        [Key]
        public int PaymentModeID { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Income> Incomes { get; set; }

    }
}
