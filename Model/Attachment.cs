using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public partial class Attachment
    {
        [Key]
        [Column("AttachmentID")]
        public int AttachmentID { get; set; }

        [Column("ExpenseID")]
        public int ExpenseID { get; set; }

        [StringLength(500)]
        [Unicode(false)]
        public string FilePath { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("ExpenseID")]
        [InverseProperty("Attachments")]
        public virtual Expense Expense { get; set; } = null!;
    }

}
