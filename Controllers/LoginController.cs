using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        // In a real application, read this from configuration.
        private readonly string _jwtSecret = "your-256-bit-secretsecretkey1234";

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the user from the custom Users table using the provided email.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null && user.PasswordHash == model.Password)
            {
                // Create the necessary claims for the authenticated user.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserID", user.UserID.ToString())
                };

                // Create a cookie-based identity using Identity's application scheme.
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Set authentication properties (e.g., persistence based on "RememberMe").
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                    // Optionally, you can add ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                // Sign in the user using the Identity.Application scheme.
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);

                // Generate a JWT token for the authenticated user.
                var jwtToken = GenerateToken(user);
                // For demonstration purposes, store the token in TempData.
                TempData["JWTToken"] = jwtToken;

                // Redirect to Home controller's Index action.
                return RedirectToAction("Index", "Home");
            }

            // If login fails, add an error message.
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user by clearing the authentication cookie.
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Optionally, clear any session data or cookies (if needed).
            TempData["LogoutMessage"] = "You have been logged out successfully.";

            // Redirect to the login page after logging out.
            return RedirectToAction("Login", "Login");
        }

        /// <summary>
        /// Generates a JWT token for the given user.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A JWT token string.</returns>
        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserID", user.UserID.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a given JWT token and returns the associated claims principal.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>A ClaimsPrincipal if valid; otherwise, null.</returns>
        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
