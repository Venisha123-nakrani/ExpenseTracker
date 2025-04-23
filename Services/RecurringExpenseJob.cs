using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class RecurringExpenseJob
    {
        private readonly ApplicationDbContext _context;
        public RecurringExpenseJob(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task RunAsync()
        {
            var today = DateTime.Today;
            int day = today.Day;

            var dueRecurringExpenses = await _context.RecurringExpenses
                .Where(r => r.IsActive && r.DateOfMonth == day &&
                            r.StartDate <= DateOnly.FromDateTime(today) &&
                            (r.EndDate == null || r.EndDate >= DateOnly.FromDateTime(today)))
                .ToListAsync();

            foreach (var rec in dueRecurringExpenses)
            {
                // Check if an expense already exists for this date and category to avoid duplicates
                bool alreadyExists = await _context.Expenses.AnyAsync(e =>
                    e.UserID == rec.UserID &&
                    e.ExpenseCategoryID == rec.CategoryID &&
                    e.ExpenseDate.Date == today &&
                    e.Amount == rec.Amount);

                if (alreadyExists) continue;

                decimal userIncome = _context.Incomes
                    .Where(i => i.UserID == rec.UserID &&
                                i.IncomeDate.Month == today.Month &&
                                i.IncomeDate.Year == today.Year)
                    .Sum(i => (decimal?)i.Amount) ?? 0;

                if (userIncome < rec.Amount)
                {
                    // Deduct from savings if income not sufficient
                    var savings = _context.Savings
                        .Where(s => s.UserID == rec.UserID && !s.IsUsed)
                        .OrderBy(s => s.Year).ThenBy(s => s.Month)
                        .ToList();

                    decimal remaining = rec.Amount;

                    foreach (var s in savings)
                    {
                        if (remaining <= 0) break;

                        if (s.Amount <= remaining)
                        {
                            remaining -= s.Amount;
                            s.IsUsed = true;
                            s.Amount = 0;
                        }
                        else
                        {
                            s.Amount -= remaining;
                            remaining = 0;
                        }

                        _context.Update(s);
                    }
                }

                // Add expense
                var newExpense = new Expense
                {
                    UserID = rec.UserID,
                    ExpenseCategoryID = rec.CategoryID,
                    Amount = rec.Amount,
                    Description = $"Auto-filled recurring expense on {today:yyyy-MM-dd}",
                    ExpenseDate = today,
                    PaymentModeID = rec.PaymentModeID,
                    CreatedAt = DateTime.Now
                };

                _context.Expenses.Add(newExpense);
            }

            await _context.SaveChangesAsync();
        }
    }
}