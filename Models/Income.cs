using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Income
    {
        [Key]
        [Column("IncomeID")]
        public int IncomeID { get; set; }
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("IncomeCategoryID")]
        public int IncomeCategoryID { get; set; }
        [Column("PaymentModeID")]
        public int PaymentModeID { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }
        public DateTime IncomeDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [ForeignKey("UserID")]
        [InverseProperty("Incomes")]
        public User User { get; set; }
        [ForeignKey("IncomeCategoryID")]
        [InverseProperty("Incomes")]
        public IncomeCategory IncomeCategory { get; set; }
        [ForeignKey("PaymentModeID")]
        [InverseProperty("Incomes")]
        public Payment Payment { get; set; }
    }
}
