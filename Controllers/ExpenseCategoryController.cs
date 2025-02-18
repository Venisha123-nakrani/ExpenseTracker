using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExpenseTracker.Controllers
{
    public class ExpenseCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExpenseCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: ExpenseCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExpenseCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] ExpenseCategory category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;

                // Check if category already exists
                var existingCategory = await _db.ExpenseCategories.FirstOrDefaultAsync(c => c.Name == category.Name);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Category already exists.");
                    return View(category);
                }

                _db.ExpenseCategories.Add(category);
                await _db.SaveChangesAsync();

                // Redirect back to Expense/Create page
                return RedirectToAction("Create", "Expense");
            }
            return View(category);
        }
    }
}
