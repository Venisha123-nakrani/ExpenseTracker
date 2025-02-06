using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsIncome { get; set; } // True = Income, False = Expense


        public User User { get; set; }
        public ExpenseCategory Category { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}
