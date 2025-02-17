using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Payment
    {

        [Key]
        [Column("PaymentModeID")]
        public int PaymentModeID { get; set; }

        [StringLength(255)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [InverseProperty("Payment")]
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
    }
}
