using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Model
{
    public class IncomeCategory
    {
        [Key]
        [Column("IncomeCategoryID")]
        public int IncomeCategoryID { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Income> Incomes { get; set; }
    }
}