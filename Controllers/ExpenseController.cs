using ExpenseTracker.Data;
using ExpenseTracker.Model;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using ExpenseTracker.Utilities;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<ExpenseController> _logger; // Logger for debugging
        private readonly ActivityLogger _activityLogger;
        public ExpenseController(ApplicationDbContext context, EmailService emailService, ILogger<ExpenseController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _activityLogger = new ActivityLogger(context);
        }

        public async Task<IActionResult> Index(string sortOrder, int? categoryId, string searchString, int pageNumber = 1)
        {
            int pageSize = 5; // Number of records per page

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

            var expenses = _context.Expenses
                .Where(e => e.UserID == user.UserID)
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .AsQueryable();

            // Populate category dropdown
            ViewData["Categories"] = new SelectList(await _context.ExpenseCategories.ToListAsync(), "ExpenseCategoryID", "Name");

            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                expenses = expenses.Where(e => e.ExpenseCategoryID == categoryId.Value);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(e =>
                    e.Description.Contains(searchString) ||
                    e.Amount.ToString().Contains(searchString) ||
                    e.Category.Name.Contains(searchString));
            }

            // Sorting logic
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "date_desc":
                    expenses = expenses.OrderByDescending(e => e.ExpenseDate);
                    break;
                case "date_asc":
                    expenses = expenses.OrderBy(e => e.ExpenseDate);
                    break;
                case "amount_desc":
                    expenses = expenses.OrderByDescending(e => e.Amount);
                    break;
                case "amount_asc":
                    expenses = expenses.OrderBy(e => e.Amount);
                    break;
                case "category_desc":
                    expenses = expenses.OrderByDescending(e => e.Category.Name);
                    break;
                case "category_asc":
                    expenses = expenses.OrderBy(e => e.Category.Name);
                    break;
                default:
                    expenses = expenses.OrderBy(e => e.ExpenseDate);
                    break;
            }

            // Pagination logic
            int totalItems = await expenses.CountAsync();
            var expenseList = await expenses.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Pass pagination details to the view
            ViewData["TotalPages"] = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewData["CurrentPage"] = pageNumber;

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(user.UserID, "Viewed expenses");

            return View(expenseList);
        }
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var expense = _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .Include(e => e.User);


            if (expense == null) return NotFound();



            return View(await expense.ToListAsync());
        }
        public IActionResult Create()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewData["UserEmail"] = userEmail ?? "";

            var categories = _context.ExpenseCategories?.ToList();
            if (categories == null || !categories.Any())
            {
                ViewBag.CategoryID = new List<SelectListItem>();
            }
            else
            {
                ViewBag.CategoryID = categories.Select(c => new SelectListItem
                {
                    Value = c.ExpenseCategoryID.ToString(),
                    Text = c.Name
                }).ToList();
            }


            ViewBag.PaymentModeID = _context.Payments?
                .Select(p => new SelectListItem
                {
                    Value = p.PaymentModeID.ToString(),
                    Text = p.Name
                })
                .ToList() ?? new List<SelectListItem>();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseID,UserID,ExpenseCategoryID,PaymentModeID,Amount,Description,ExpenseDate")] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                LoadViewData(expense);
                return View(expense);
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            expense.UserID = _context.Users.FirstOrDefault(u => u.Email == userEmail)?.UserID ?? 0;

            int expenseMonth = expense.ExpenseDate.Month;
            int expenseYear = expense.ExpenseDate.Year;

            var totalBudget = _context.Budgets
                .Where(b => b.CategoryID == expense.ExpenseCategoryID && b.Year == expenseYear && (b.Month == 0 || b.Month == expenseMonth))
                .Sum(b => (decimal?)b.Amount) ?? 0;

            var totalSpent = _context.Expenses
                .Where(e => e.ExpenseCategoryID == expense.ExpenseCategoryID && e.ExpenseDate.Year == expenseYear && e.ExpenseDate.Month == expenseMonth)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            decimal remainingBudget = totalBudget - totalSpent;

            if (totalBudget > 0)
            {
                if (expense.Amount > remainingBudget)
                {
                    TempData["ErrorMessage"] = $"Error: You cannot exceed the total budget of {totalBudget:C} for this category. Remaining budget: {remainingBudget:C}";
                    LoadViewData(expense);
                    return View(expense);
                }
                if (expense.Amount > (0.7m * totalBudget) && remainingBudget > expense.Amount)
                {
                    TempData["WarningMessage"] = $"Warning: This expense exceeds 70% of your budget for this category!";
                }
            }

            expense.CreatedAt = DateTime.Now;
            _context.Add(expense);
            await _context.SaveChangesAsync();

            decimal newRemainingBudget = remainingBudget - expense.Amount;
            if (newRemainingBudget <= 0)
            {
                try
                {
                    string emailSubject = "⚠️ Budget Alert: You Have Exhausted Your Budget";
                    string emailBody = $@"
                    <h3>Dear User,</h3>
                    <p>You have used up your total budget of <b>{totalBudget:C}</b> for this category.</p>
                    <p>Any further expenses in this category will exceed your set budget.</p>
                    <p>Please review your budget and plan accordingly.</p>
                    <br/>
                    <p>Thank you,</p>
                    <p><b>Expense Management System</b></p>";

                    _logger.LogInformation("Sending budget exhausted email to {Email}", userEmail);
                    await _emailService.SendEmailAsync(userEmail, emailSubject, emailBody);
                    _logger.LogInformation("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Email sending failed: {ErrorMessage}", ex.Message);
                }

            }

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(expense.UserID, "Created a new expense");

            return RedirectToAction(nameof(Index));
        }
        private void LoadViewData(Expense expense)
        {
            ViewData["CategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", expense.ExpenseCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", expense.PaymentModeID);
            ViewData["UserEmail"] = HttpContext.Session.GetString("UserEmail");
        }

        [HttpGet]
        // API: Get Budget & Total Spent for Selected Category, Month & Year

        public IActionResult GetCategoryBudget(int categoryId, int year, int month)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { success = false, message = "Session expired. Please log in again." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Get total allocated budget for the category in the selected month & year
            decimal totalBudget = _context.Budgets
                .Where(b => b.UserID == user.UserID && b.CategoryID == categoryId && b.Year == year && b.Month == month)
                .Sum(b => b.Amount);

            // Get total expenses already made in this category for selected month & year
            decimal totalSpent = _context.Expenses
                .Where(e => e.UserID == user.UserID && e.ExpenseCategoryID == categoryId && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)
                .Sum(e => e.Amount);

            return Json(new { success = true, budget = totalBudget, spent = totalSpent });
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewData["CategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", expense.ExpenseCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", expense.PaymentModeID);

            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewData["UserEmail"] = userEmail;
            return View(expense);


        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseID,UserID,ExpenseCategoryID,PaymentModeID,Amount,Description,ExpenseDate")] Expense expense)
        {
            if (id != expense.ExpenseID) return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    var userEmail = HttpContext.Session.GetString("UserEmail");
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                    // Assign the correct UserID before saving
                    expense.UserID = user.UserID;
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Expenses.Any(e => e.ExpenseID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));

            }

            ViewData["CategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", expense.ExpenseCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", expense.PaymentModeID);

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(expense.UserID, "Updated an expense");

            return View(expense);

        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .FirstOrDefaultAsync(m => m.ExpenseID == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
            // ✅ Log the activity
            await _activityLogger.LogUserActivity(expense.UserID, "Deleted an expense");

            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseID == id);

            if (expense == null) return NotFound();

            // ✅ Log the activity
            await _activityLogger.LogUserActivity(expense.UserID, "Viewed expense details");

            return View(expense);
        }




    }
}