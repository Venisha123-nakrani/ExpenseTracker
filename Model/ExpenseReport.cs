using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class ExpenseReport
    {
        [Key]
        public int ExpenseReportID { get; set; }
        public int UserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }

    }
}
