using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
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
            // Fetch logged-in user email from session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewData["UserEmail"] = userEmail; // Store in ViewData to display in the view

            ViewBag.CategoryID = _context.ExpenseCategories
                .Select(c => new SelectListItem
                {
                    Value = c.ExpenseCategoryID.ToString(),
                    Text = c.Name
                })
                .ToList()
                .DistinctBy(c => c.Text)
                .ToList();

            ViewBag.PaymentModeID = _context.Payments
                .Select(p => new SelectListItem
                {
                    Value = p.PaymentModeID.ToString(),
                    Text = p.Name
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseID,UserID,ExpenseCategoryID,PaymentModeID,Amount,Description,ExpenseDate")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.UserID = (int)(_context.Users.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("UserEmail"))?.UserID);

                expense.CreatedAt = DateTime.Now;
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the ViewData for dropdowns to retain selections in case of validation errors
            ViewData["CategoryID"] = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", expense.ExpenseCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", expense.PaymentModeID);
            // Retain entered UserID value if the form is not valid
            ViewData["UserID"] = expense.UserID;

            return View(expense);


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
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseID == id);

            if (expense == null) return NotFound();

            return View(expense);
        }



    }
}