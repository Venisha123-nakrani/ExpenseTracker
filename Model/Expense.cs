using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public partial class Expense
    {
        [Key]
        [Column("ExpenseID")]
        public int ExpenseID { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }

        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Column("PaymentModeID")]
        public int PaymentModeID { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }
        //  public bool IsIncome { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        public DateOnly ExpenseDate { get; set; }

        [Column(TypeName = "datetime")]

        public DateTime? CreatedAt { get; set; }

        [InverseProperty("Expense")]
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        [ForeignKey("CategoryID")]
        [InverseProperty("Expenses")]
        public virtual ExpenseCategory Category { get; set; } = null!;

        [ForeignKey("UserID")]
        [InverseProperty("Expenses")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("PaymentModeID")]
        [InverseProperty("Expenses")]
        public virtual Payment Payment { get; set; } = null!;

    }

}
