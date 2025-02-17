//using ExpenseTracker.Data;
//using ExpenseTracker.Model;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;

//namespace ExpenseTracker.Controllers
//{
//    public class IncomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public IncomeController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Income
//        public async Task<IActionResult> Index()
//        {
//            // Check if the user is logged in
//            if (!User.Identity.IsAuthenticated)
//            {
//                TempData["ErrorMessage"] = "You must be logged in to view this page.";
//                return RedirectToAction("Login", "Login");  // Redirect to Login page
//            }

//            var incomes = _context.Incomes.Include(i => i.IncomeCategory).Include(i => i.User).Include(i => i.Payment);
//            return View(await incomes.ToListAsync());
//        }

//        // GET: Income/Create
//        public IActionResult Create()
//        {
//            // Check if the user is logged in
//            if (!User.Identity.IsAuthenticated)
//            {
//                TempData["ErrorMessage"] = "You must be logged in to create income records.";
//                return RedirectToAction("Login", "Account");  // Redirect to Login page
//            }

//            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName");
//            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name");
//            return View();
//        }

//        // POST: Income/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("IncomeID,UserID,IncomeCategoryID,PaymentModeID,Amount,Description,IncomeDate")] Income income)
//        {
//            // Check if the user is logged in
//            if (!User.Identity.IsAuthenticated)
//            {
//                TempData["ErrorMessage"] = "You must be logged in to create income records.";
//                return RedirectToAction("Login", "Account");  // Redirect to Login page
//            }

//            if (ModelState.IsValid)
//            {
//                income.CreatedAt = DateTime.Now;
//                _context.Add(income);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            // Re-populate the ViewData for dropdowns to retain selections in case of validation errors
//            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName", income.IncomeCategoryID);
//            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", income.PaymentModeID);
//            // Retain entered UserID value if the form is not valid
//            ViewData["UserID"] = income.UserID;

//            return View(income);
//        }
//    }
//}
using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to view this page.";
                return RedirectToAction("Login", "Account");
            }

            var incomes = _context.Incomes.Include(i => i.IncomeCategory).Include(i => i.User).Include(i => i.Payment);
            return View(await incomes.ToListAsync());
        }

        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to create income records.";
                return RedirectToAction("Login", "Account");
            }

            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName");
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name");
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

            if (ModelState.IsValid)
            {
                income.CreatedAt = DateTime.Now;
                _context.Add(income);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IncomeCategoryID"] = new SelectList(_context.IncomeCategories, "IncomeCategoryID", "CategoryName", income.IncomeCategoryID);
            ViewData["PaymentModeID"] = new SelectList(_context.Payments, "PaymentModeID", "Name", income.PaymentModeID);
            ViewData["UserID"] = income.UserID;
            return View(income);
        }

        // Other CRUD actions would go here
    }
}
