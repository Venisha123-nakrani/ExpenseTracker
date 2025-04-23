using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class RecurringExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RecurringExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            var list = _context.RecurringExpenses
                .Include(r => r.Category)
                .Include(r => r.Payment)
                .Where(r => r.UserID == user.UserID)
                .ToList();

            return View(list);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name");
            ViewBag.Payments = new SelectList(_context.Payments, "PaymentModeID", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RecurringExpense recurring)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", recurring.CategoryID);
                ViewBag.Payments = new SelectList(_context.Payments, "PaymentModeID", "Name", recurring.PaymentModeID);
                return View(recurring);
            }

            recurring.UserID = user.UserID;
            recurring.CreatedAt = DateTime.Now;
            _context.Add(recurring);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var recurring = _context.RecurringExpenses.Find(id);
            if (recurring == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", recurring.CategoryID);
            ViewBag.Payments = new SelectList(_context.Payments, "PaymentModeID", "Name", recurring.PaymentModeID);
            return View(recurring);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RecurringExpense recurring)
        {
            if (id != recurring.RecurringID) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.ExpenseCategories, "ExpenseCategoryID", "Name", recurring.CategoryID);
                ViewBag.Payments = new SelectList(_context.Payments, "PaymentModeID", "Name", recurring.PaymentModeID);
                return View(recurring);
            }

            var existing = _context.RecurringExpenses.Find(id);
            if (existing == null) return NotFound();

           // existing.CategoryID = recurring.CategoryID;
            existing.PaymentModeID = recurring.PaymentModeID;
            existing.Amount = recurring.Amount;
            existing.StartDate = recurring.StartDate;
            existing.EndDate = recurring.EndDate;
            existing.DateOfMonth = recurring.DateOfMonth;
            existing.IsActive = recurring.IsActive;

            _context.Update(existing);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var recurring = _context.RecurringExpenses
                .Include(r => r.Category)
                .Include(r => r.Payment)
                .FirstOrDefault(r => r.RecurringID == id);

            if (recurring == null) return NotFound();

            return View(recurring);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var recurring = _context.RecurringExpenses.Find(id);
            if (recurring == null) return NotFound();

            _context.RecurringExpenses.Remove(recurring);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var recurring = _context.RecurringExpenses.Find(id);
            if (recurring != null)
            {
                recurring.IsActive = !recurring.IsActive;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
