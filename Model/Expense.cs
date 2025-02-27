using System;
using System.Collections.Generic;
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

        [Column("ExpenseCategoryID")] // Updated to match the database schema
        public int ExpenseCategoryID { get; set; }

        [Column("PaymentModeID")]
        public int PaymentModeID { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Updated precision to match the database
        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }

        [Column(TypeName = "datetime2(7)")]
        public DateTime ExpenseDate { get; set; }

        [Column(TypeName = "datetime2(7)")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [InverseProperty("Expense")]
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        // Foreign key relationships
        [ForeignKey("ExpenseCategoryID")]
        [InverseProperty("Expenses")]
        public virtual ExpenseCategory? Category { get; set; } = null!;

        [ForeignKey("UserID")]
        [InverseProperty("Expenses")]
        public virtual User? User { get; set; } = null!;

        [ForeignKey("PaymentModeID")]
        [InverseProperty("Expenses")]
        public virtual Payment? Payment { get; set; } = null!;
    }
}
