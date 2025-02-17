using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class IncomeCategory
    {
        public int IncomeCategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        public string CategoryName { get; set; }

        public ICollection<Income> Incomes { get; set; }
    }
}