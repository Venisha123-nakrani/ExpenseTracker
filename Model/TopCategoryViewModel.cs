namespace ExpenseTracker.Model
{
    public class TopCategoryViewModel
    {
        public List<string> CategoryNames { get; set; } = new();
        public List<decimal> CategoryTotals { get; set; } = new();
    }
}
