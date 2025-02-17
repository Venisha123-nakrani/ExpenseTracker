using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ExpenseTrackerDbContext _db;

        public RegistrationController(ExpenseTrackerDbContext db)
        {
            _db = db;
        }

        // GET: Registration Page
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registration Form Submission
        [HttpPost]
        public IActionResult Create(User model, string cpassword)
        {
            // Remove validation for PasswordHash since it will be computed manually
            ModelState.Remove("PasswordHash");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            if (_db.User.Any(u => u.Email == model.Email.Trim()))
            {
                ModelState.AddModelError("Email", "Email already in use");
                return View(model);
            }

            // Validate Password & Confirm Password match
            if (model.Password != cpassword)
            {
                ModelState.AddModelError("cpassword", "Passwords do not match");
                return View(model);
            }

            // Generate Salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash Password
            model.PasswordHash = HashPassword(model.Password, salt);

            // Remove raw password before saving to DB
            model.Password = null;

            // Save User to Database
            _db.User.Add(model);
            _db.SaveChanges();

            TempData["Message"] = "Registration successful";
            return RedirectToAction("Index","Expense");
        }
        private string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}
