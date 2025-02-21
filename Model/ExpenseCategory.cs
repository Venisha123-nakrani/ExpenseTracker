using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public partial class ExpenseCategory
    {
        [Key]
        //[Column("CategoryID")]
        public int ExpenseCategoryID { get; set; }

        //[StringLength(255)]
        //[Unicode(false)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        //[InverseProperty("Category")]
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

        //[InverseProperty("Category")]
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

       // [InverseProperty("Category")]
        public virtual ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
    }

}
