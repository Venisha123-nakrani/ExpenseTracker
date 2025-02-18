using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Model;
using System.Linq;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public RegistrationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User model, string cpassword)
        {
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Expenses");
            ModelState.Remove("ExpenseReports");
            ModelState.Remove("RecurringExpenses");
            ModelState.Remove("Budgets");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_db.Users.Any(u => u.Email == model.Email.Trim()))
            {
                ModelState.AddModelError("Email", "Email already in use");
                return View(model);
            }

            if (model.Password != cpassword)
            {
                ModelState.AddModelError("cpassword", "Passwords do not match");
                return View(model);
            }

            // Hash password using PasswordHasher
            model.PasswordHash = _passwordHasher.HashPassword(model, model.Password);

            model.Password = null; // Remove raw password before saving

            _db.Users.Add(model);
            _db.SaveChanges();

            TempData["Message"] = "Registration successful";
            return RedirectToAction("Login", "Login");
        }
    }
}
