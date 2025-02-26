using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class IncomeCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IncomeCategory
        public async Task<IActionResult> Index()
        {
            return View(await _context.IncomeCategories.ToListAsync());
        }


        // GET: IncomeCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IncomeCategory/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName")] IncomeCategory incomeCategory)
        {
            if (!ModelState.IsValid)
            {
                _context.IncomeCategories.Add(incomeCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(incomeCategory);
        }


        // GET: IncomeCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeCategory = await _context.IncomeCategories.FindAsync(id);
            if (incomeCategory == null)
            {
                return NotFound();
            }
            return View(incomeCategory);
        }

        // POST: IncomeCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IncomeCategoryID,CategoryName")] IncomeCategory incomeCategory)
        {
            if (id != incomeCategory.IncomeCategoryID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(incomeCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomeCategoryExists(incomeCategory.IncomeCategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(incomeCategory);
        }

        private bool IncomeCategoryExists(int id)
        {
            return _context.IncomeCategories.Any(e => e.IncomeCategoryID == id);
        }



        // GET: IncomeCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeCategory = await _context.IncomeCategories
                .FirstOrDefaultAsync(m => m.IncomeCategoryID == id);
            if (incomeCategory == null)
            {
                return NotFound();
            }

            return View(incomeCategory);
        }

        // POST: IncomeCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incomeCategory = await _context.IncomeCategories.FindAsync(id);
            if (incomeCategory != null)
            {
                _context.IncomeCategories.Remove(incomeCategory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
