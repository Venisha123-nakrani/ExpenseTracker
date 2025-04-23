using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class Saving
    {
        [Key]
        public int SavingID { get; set; }

        public int UserID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        [InverseProperty("Savings")]
        public virtual User User { get; set; } = null!;
    }

}
