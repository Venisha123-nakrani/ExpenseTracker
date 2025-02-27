using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public partial class Budget
    {
        [Key]
        [Column("BudgetID")]
        public int BudgetID { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("CategoryID")]
        [InverseProperty("Budgets")]
        public virtual ExpenseCategory Category { get; set; } = null!;

        [ForeignKey("UserID")]
        [InverseProperty("Budgets")]
        public virtual User User { get; set; } = null!;
    }
}
