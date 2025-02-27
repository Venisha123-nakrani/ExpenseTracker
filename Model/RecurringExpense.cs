using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public partial class RecurringExpense
    {
        [Key]
        [Column("RecurringID")]
        public int RecurringID { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Frequency { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("CategoryID")]
        [InverseProperty("RecurringExpenses")]
        public virtual ExpenseCategory Category { get; set; } = null!;

        [ForeignKey("UserID")]
        [InverseProperty("RecurringExpenses")]
        public virtual User User { get; set; } = null!;
    }

}
