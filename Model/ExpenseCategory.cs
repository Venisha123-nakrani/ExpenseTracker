using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class ExpenseCategory
    {
        [Key]
        public int ExpenseCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<RecurringExpense> RecurringExpenses { get; set; }
    }
}
