﻿
using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;
        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, int? categoryId, string searchString, int pageNumber = 1)
        {
            int pageSize = 5; // Number of records per page

            // Get the logged-in user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid user. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Get incomes for the logged-in user
            var incomes = _context.Incomes
                .Where(i => i.UserID == user.UserID)
                .Include(i => i.IncomeCategory)
                .Include(i => i.Payment)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                incomes = incomes.Where(i => i.Description.Contains(searchString) || i.IncomeCategory.CategoryName.Contains(searchString));
            }

            // Category filter
            if (categoryId.HasValue)
            {
                incomes = incomes.Where(i => i.IncomeCategoryID == categoryId.Value);
            }

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "date_desc":
                    incomes = incomes.OrderByDescending(i => i.IncomeDate);
                    break;
                case "date_asc":
                    incomes = incomes.OrderBy(i => i.IncomeDate);
                    break;
                case "amount_desc":
                    incomes = incomes.OrderByDescending(i => i.Amount);
                    break;
                case "amount_asc":
                    incomes = incomes.OrderBy(i => i.Amount);
                    break;
                case "category_desc":
                    incomes = incomes.OrderByDescending(i => i.IncomeCategory.CategoryName);
                    break;
                case "category_asc":
                    incomes = incomes.OrderBy(i => i.IncomeCategory.CategoryName);
                    break;
                default:
                    incomes = incomes.OrderBy(i => i.IncomeDate);
                    break;
            }

            // Pagination
            int totalItems = await incomes.CountAsync();
            var incomeList = await incomes.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Populate dropdown for income categories
            ViewData["Categories"] = new SelectList(await _context.IncomeCategories.ToListAsync(), "IncomeCategoryID", "CategoryName");

            // Pass pagination details to the view
            ViewData["TotalPages"] = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewData["CurrentPage"] = pageNumber;

            return View(incomeList);
        }
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to create income records.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch data for the dropdowns
            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName");
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name");

            // Fetch the logged-in user's email from the session or claims
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewData["UserEmail"] = userEmail;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IncomeID,UserID,IncomeCategoryID,PaymentModeID,Amount,Description,IncomeDate")] Income income)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to create income records.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Set UserID from the logged-in user's email (or session)
                income.UserID = (int)(_context.Users.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("UserEmail"))?.UserID);

                income.CreatedAt = DateTime.Now;
                _context.Add(income);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the dropdown lists in case of validation errors
            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName", income.IncomeCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", income.PaymentModeID);

            return View(income);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var income = await _context.Incomes.FindAsync(id);
            if (income == null) return NotFound();

            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName", income.IncomeCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", income.PaymentModeID);

            // Fetch the logged-in user's email from the session
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewData["UserEmail"] = userEmail;

            return View(income);
        }


        // POST: Income/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IncomeID,IncomeCategoryID,PaymentModeID,Amount,Description,IncomeDate")] Income income)
        {
            if (id != income.IncomeID) return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    // Fetch the logged-in user's email
                    var userEmail = HttpContext.Session.GetString("UserEmail");
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Invalid user. Please log in again.";
                        return RedirectToAction("Login", "Account");
                    }

                    // Assign the correct UserID before saving
                    income.UserID = user.UserID;

                    _context.Update(income);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Incomes.Any(e => e.IncomeID == id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns to prevent errors on returning view
            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName", income.IncomeCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", income.PaymentModeID);

            return View(income);
        }


        // GET: Income/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var income = await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(m => m.IncomeID == id);
            if (income == null) return NotFound();

            return View(income);
        }

        // POST: Income/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var income = await _context.Incomes.FindAsync(id);
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Income/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the income details along with related data (IncomeCategory, User, Payment)
            var income = await _context.Incomes
                .Include(i => i.IncomeCategory)
                .Include(i => i.User)
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(m => m.IncomeID == id);

            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

    }
}