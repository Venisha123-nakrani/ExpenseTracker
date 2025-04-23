using ExpenseTracker.Data;
using ExpenseTracker.Model;
using ExpenseTracker.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<User> _userManager;
        private readonly ActivityLogger _activityLogger;
        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
            _activityLogger = new ActivityLogger(context);

        }

        public async Task<IActionResult> Index()
        {  // Get the logged-in user's email from claims
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect if user is not logged in
            }

            // Fetch the user details from the database
            var user = await _context.Users
                .Where(u => u.Email == userEmail)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.ImagePath = string.IsNullOrEmpty(user.ImagePath) ? "/assets_db/img/profile-img.jpg" : user.ImagePath;

            // Log user activity
            await _activityLogger.LogUserActivity(user.UserID, "Viewed profile");

            return View(user); // Pass user data to the View
        }
        public async Task<IActionResult> Edit()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User updatedUser, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                return View(updatedUser);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == updatedUser.Email);
            if (user == null)
            {
                return NotFound();
            }

            // Update editable fields
            user.FullName = updatedUser.FullName;
            user.BirthDate = updatedUser.BirthDate;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Address = updatedUser.Address;
            user.Country = updatedUser.Country;

            // Handle Image Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = "wwwroot/uploads";
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to wwwroot/uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save the relative path to database
                user.ImagePath = "/uploads/" + uniqueFileName;
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            // Log user activity
            await _activityLogger.LogUserActivity(user.UserID, "Updated profile");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var model = new ChangePasswordViewModel { Email = user.Email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return NotFound("User not found");

            // ✅ Verify old password using BCrypt
            if (!PasswordHasher.PasswordVerify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return View(model);
            }

            // ✅ Hash new password using BCrypt
            user.PasswordHash = PasswordHasher.PasswordHash(model.NewPassword);

            _context.Update(user);
            await _context.SaveChangesAsync();

            await _activityLogger.LogUserActivity(user.UserID, "Changed password");

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }


    }
}
