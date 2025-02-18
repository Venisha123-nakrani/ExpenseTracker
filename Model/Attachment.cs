using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class Attachment
    {
        [Key]
        public int AttachmentID { get; set; }
        public int ExpenseID { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Expense Expense { get; set; }

    }
}
