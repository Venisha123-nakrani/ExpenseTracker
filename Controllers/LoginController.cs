using ExpenseTracker.Data;
using ExpenseTracker.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace ExpenseTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly string _jwtSecret;

        public LoginController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _jwtSecret = _configuration["Jwt:Key"]; // Load secret key from config
        }

        [HttpGet]
        public IActionResult Login()
        {
            // 🔹 Restore session if lost but user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    var userEmail = User.FindFirstValue(ClaimTypes.Email);
                    var userImage = User.FindFirstValue("UserImage") ?? "/images/default-user.png";

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        HttpContext.Session.SetString("UserEmail", userEmail);
                        HttpContext.Session.SetString("UserImage", userImage);
                    }
                }
                return RedirectToAction("Dashboard", "Home");
            }
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserID", user.UserID.ToString()),
                new Claim("UserImage", user.ImagePath ?? "/images/default-user.png") // 🔹 Store image path in claims
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // 🔹 30-minute session expiration
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            // 🔹 Maintain session for user authentication
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserImage", user.ImagePath ?? "/images/default-user.png");

            // 🔹 Generate and store JWT Token
            var jwtToken = GenerateToken(user);
            TempData["JWTToken"] = jwtToken;

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("AuthToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            // Prevent browser from caching authentication
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Login");
        }

        /// <summary>
        /// 🔹 Generates a JWT token for the authenticated user.
        /// </summary>
        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserID", user.UserID.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30), // 🔹 Token expires in 30 minutes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 🔹 Validates a JWT token and extracts the claims.
        /// </summary>
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

        [HttpGet]
        public IActionResult CheckSession()
        {
            return Json(new { isAuthenticated = User.Identity.IsAuthenticated });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View();
            }

            // Generate token
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.TokenExpiryTime = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // Send Email
            var resetLink = Url.Action("ResetPassword", "Login", new { email = user.Email, token = token }, Request.Scheme);
            var body = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            // Replace with real SMTP logic
            await SendEmail(user.Email, "Password Reset", body);

            ViewBag.Message = "Password reset link has been sent to your email.";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.PasswordResetToken != model.Token || user.TokenExpiryTime < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Invalid or expired token.");
                return View();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            user.PasswordResetToken = null;
            user.TokenExpiryTime = null;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password reset successful. You can now login.";
            return RedirectToAction("Login");
        }

        // Example SMTP method (use your config)
        private async Task SendEmail(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("hajariwalagracy@gmail.com", "tkpz mzdo rugu epep"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress("hajariwalagracy@gmail.com", "Expense Tracker"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);
            await client.SendMailAsync(mail);
        }

    }

}
