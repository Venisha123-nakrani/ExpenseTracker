using ExpenseTracker.Data;
using ExpenseTracker.Model;
using ExpenseTracker.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogger _activityLogger;
        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
            _activityLogger = new ActivityLogger(context);
        }

        public async Task<IActionResult> Index(string sortOrder, int? categoryId, int? month, int? year, int pageNumber = 1)
        {
            int pageSize = 4; // Number of records per page

            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to view this page.";
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid user. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var budgets = _context.Budgets
                .Where(b => b.UserID == user.UserID)
                .Include(b => b.Category)
                .AsQueryable();

            // Populate category dropdown
            ViewData["Categories"] = new SelectList(await _context.ExpenseCategories.ToListAsync(), "ExpenseCategoryID", "Name");
            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                budgets = budgets.Where(b => b.CategoryID == categoryId.Value);
            }

            // Apply month filter
            if (month.HasValue && month.Value > 0)
            {
                budgets = budgets.Where(b => b.Month == month.Value);
            }

            // Apply year filter
            if (year.HasValue && year.Value > 0)
            {
                budgets = budgets.Where(b => b.Year == year.Value);
            }

            // Sorting logic
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "amount_desc":
                    budgets = budgets.OrderByDescending(b => b.Amount);
                    break;
                case "amount_asc":
                    budgets = budgets.OrderBy(b => b.Amount);
                    break;
                case "category_desc":
                    budgets = budgets.OrderByDescending(b => b.Category.Name);
                    break;
                case "category_asc":
                    budgets = budgets.OrderBy(b => b.Category.Name);
                    break;
                case "month_desc":
                    budgets = budgets.OrderByDescending(b => b.Month);
                    break;
                case "month_asc":
                    budgets = budgets.OrderBy(b => b.Month);
                    break;
                case "year_desc":
                    budgets = budgets.OrderByDescending(b => b.Year);
                    break;
                case "year_asc":
                    budgets = budgets.OrderBy(b => b.Year);
                    break;
                default:
                    budgets = budgets.OrderBy(b => b.Year).ThenBy(b => b.Month);
                    break;
            }

            // Pagination logic
            int totalItems = await budgets.CountAsync();
            var budgetList = await budgets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Pass pagination details to the view
            ViewData["TotalPages"] = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewData["CurrentPage"] = pageNumber;

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(user.UserID, "Viewed budget list");

            return View(budgetList);
        }


        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a budget.";
                return RedirectToAction("Login", "Account");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .Include(u => u.Incomes)
                .FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int currentMonth = DateTime.Now.Month;

            decimal currentMonthIncome = user.Incomes
                .Where(i => i.IncomeDate.Month == currentMonth && i.IncomeDate.Year == currentYear)
                .Sum(i => i.Amount);

            ViewData["UserEmail"] = userEmail;
            ViewData["UserIncome"] = currentMonthIncome;

            ViewBag.ExpenseCategoryID = _context.ExpenseCategories
                .Select(c => new SelectListItem
                {
                    Value = c.ExpenseCategoryID.ToString(),
                    Text = c.Name
                })
                .ToList() ?? new List<SelectListItem>();

            return View();
        }

        // API: Get Income for Selected Month & Year
        [HttpGet]
        public IActionResult GetIncomeForMonthYear(int year, int month)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Session expired. Please log in again." });
            }

            var user = _context.Users
                .Include(u => u.Incomes)
                .FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            decimal selectedMonthIncome = user.Incomes
                .Where(i => i.IncomeDate.Month == month && i.IncomeDate.Year == year)
                .Sum(i => i.Amount);

            return Json(new { success = true, income = selectedMonthIncome });
        }

        // POST: Create Budget
        // POST: Create Budget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetID,UserID,CategoryID,Amount,Month,Year")] Budget budget)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a budget.";
                return RedirectToAction("Login", "Account");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                      .Include(u => u.Incomes)
                      .Include(u => u.Expenses)
                      .FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            budget.UserID = user.UserID;
            budget.CreatedAt = DateTime.Now;

            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int currentMonth = DateTime.Now.Month;

            // Ensure the year is valid
            if (budget.Year < currentYear || budget.Year > nextYear)
            {
                TempData["ErrorMessage"] = "Invalid year selection. You can only create a budget for the current or next year.";
                return RedirectToAction(nameof(Create));
            }

            // Ensure the month is valid (current or future, not past)
            if (budget.Year == currentYear && budget.Month < currentMonth)
            {
                TempData["ErrorMessage"] = "You cannot set a budget for a past month.";
                return RedirectToAction(nameof(Create));
            }

            // **Check if a budget for this category, month, and year already exists**
            var existingBudget = _context.Budgets.FirstOrDefault(b =>
                b.UserID == user.UserID &&
                b.CategoryID == budget.CategoryID &&
                b.Month == budget.Month &&
                b.Year == budget.Year);

            if (existingBudget != null)
            {
                TempData["ErrorMessage"] = "A budget for this category already exists for the selected month and year.";
                return RedirectToAction(nameof(Create));
            }

            // Get total income for selected month and year
            decimal totalIncome = user.Incomes
                .Where(i => i.IncomeDate.Month == budget.Month && i.IncomeDate.Year == budget.Year)
                .Sum(i => i.Amount);

            // Get total allocated budget
            decimal totalAllocatedBudget = _context.Budgets
                .Where(b => b.UserID == user.UserID && b.Year == budget.Year && b.Month == budget.Month)
                .Sum(b => b.Amount);

            decimal availableBudget = totalIncome - totalAllocatedBudget;

            if (budget.Amount > availableBudget)
            {
                TempData["ErrorMessage"] = $"You cannot set a budget more than your income. Your available income is ₹{availableBudget}.";
                return RedirectToAction(nameof(Create));
            }

            _context.Add(budget);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Budget created successfully!";

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(user.UserID, "Created a budget");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BudgetID == id);

            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            ViewData["ExpenseCategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", budget.CategoryID);

            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewData["UserEmail"] = userEmail;

            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetID,UserID,CategoryID,Amount,Month,Year")] Budget budget)
        {

            if (!ModelState.IsValid)
            {
                try
                {
                    var userEmail = HttpContext.Session.GetString("UserEmail");
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Invalid user. Please log in again.";
                        return RedirectToAction("Login", "Account");
                    }

                    budget.UserID = user.UserID;

                    _context.Update(budget);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Budgets.Any(e => e.BudgetID == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExpenseCategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", budget.CategoryID);

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(budget.UserID, "Updated a budget");

            return View(budget);



        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                TempData["ErrorMessage"] = "Budget not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Budget deleted successfully.";

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(budget.UserID, "Deleted a budget");

            return RedirectToAction(nameof(Index));
        }
        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.BudgetID == id);
        }
    }
}
