using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class UserActivity
    {
        [Key]
        public int ActivityID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string Action { get; set; } // e.g., "Created Budget", "Updated Expense"

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        [InverseProperty("UserActivities")]
        public virtual User? User { get; set; } = null!;
    }
}
