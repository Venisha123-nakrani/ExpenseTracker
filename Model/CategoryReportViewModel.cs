using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Model
{
    public class CategoryReportViewModel
    {
        public int SelectedCategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }

        public List<ExpenseDto> Expenses { get; set; }
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
    }
    public class ExpenseDto
    {
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int ExpenseID { get; internal set; }
    }
}