using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ExpenseTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
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



            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && user.PasswordHash == model.Password)

            {

                var claims = new List<Claim>

        {

            new Claim(ClaimTypes.Name, user.FullName),

            new Claim(ClaimTypes.Email, user.Email),

            new Claim("UserID", user.UserID.ToString())

        };



                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

                var principal = new ClaimsPrincipal(identity);



                var authProperties = new AuthenticationProperties

                {

                    IsPersistent = model.RememberMe

                };



                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);



                // Store the logged-in user's email in the session

                HttpContext.Session.SetString("UserEmail", user.Email);



                var jwtToken = GenerateToken(user);

                TempData["JWTToken"] = jwtToken;



                return RedirectToAction("Index", "Home");

            }



            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Clear session data on logout

            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "You have been logged out successfully.";
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
