using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ExpenseTrackerDbContext _db;
        public ExpenseController(ExpenseTrackerDbContext db) 
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var expenses = await _db.Expense
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .Include(e => e.User)
                .ToListAsync();

            return View(expenses);
        }
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var expense = _db.Expense
                .Include(e => e.Category)
                .Include(e => e.Payment)
                .Include(e => e.User);


            if (expense == null) return NotFound();

            return View(await expense.ToListAsync());
        }
        public IActionResult Create()
        {

            ViewBag.CategoryID = _db.ExpenseCategory
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryID.ToString(),
                    Text = c.Name
                })
                .ToList() 
                .DistinctBy(c => c.Text) 
                .ToList(); 

            ViewBag.PaymentModeID = _db.Payment
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
        public async Task<IActionResult> Create([Bind("ExpenseID,UserID,CategoryID,PaymentModeID,Amount,Description,ExpenseDate")] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                expense.CreatedAt = DateTime.Now;
                _db.Add(expense);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the ViewData for dropdowns to retain selections in case of validation errors
            ViewData["CategoryID"] = new SelectList(_db.ExpenseCategory, "CategoryID", "Name", expense.CategoryID);
            ViewData["PaymentModeID"] = new SelectList(_db.Payment, "PaymentModeID", "Name", expense.PaymentModeID);
            // Retain entered UserID value if the form is not valid
            ViewData["UserID"] = expense.UserID;

            return View(expense);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _db.Expense.FindAsync(id);
            if (expense == null) return NotFound();

            ViewData["CategoryID"] = new SelectList(_db.ExpenseCategory, "CategoryID", "Name", expense.CategoryID);
            ViewData["PaymentModeID"] = new SelectList(_db.Payment, "PaymentModeID", "Name", expense.PaymentModeID);
            return View(expense);
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseID,UserID,CategoryID,PaymentModeID,Amount,Description,ExpenseDate")] Expense expense)
        {
            if (id != expense.ExpenseID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(expense);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.Expense.Any(e => e.ExpenseID == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["CategoryID"] = new SelectList(_db.ExpenseCategory, "CategoryID", "Name", expense.CategoryID);
            ViewData["PaymentModeID"] = new SelectList(_db.Payment, "PaymentModeID", "Name", expense.PaymentModeID);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _db.Expense
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
            var expense = await _db.Expense.FindAsync(id);
            if (expense != null)
            {
                _db.Expense.Remove(expense);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}