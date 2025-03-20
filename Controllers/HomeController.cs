using CsvHelper;
using ExpenseTracker.Data;
using ExpenseTracker.Model;
using ExpenseTracker.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using System.Security.Authentication;
using System.Text.Json;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly GmailImapService _gmailImapService;

        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger, ApplicationDbContext context, GmailImapService gmailImapService)
        {
            _logger = logger;
            _context = context;
            _gmailImapService = gmailImapService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {


            return View();
        }
      
        public async Task<IActionResult> Dashboard(string startDate = null, string endDate = null, int page = 1, int pageSize = 5)
        {
            // Retrieve the logged-in user's email from session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login"); // Redirect if not logged in
            }

            // Fetch user details from the database
            var user = _context.Users
                .Where(u => u.Email == userEmail)
                .Select(u => new { u.Email, u.ImagePath, u.UserID })
                .FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Login", "Login"); // Redirect if user not found
            }

            // **Securely Fetch App Password**
            string appPassword = _configuration["GmailAppPassword"];

            try
            {
                var gmailNotifications = await _gmailImapService.GetUserGmailNotificationsAsync(userEmail, appPassword);
                ViewBag.GmailNotifications = gmailNotifications;
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError("Authentication failed: {ErrorMessage}", ex.Message);
                ViewBag.GmailNotifications = new List<GmailNotification>
            {
                new GmailNotification
                {
                    Subject = "Authentication Error",
                    Sender = "System",
                    DateReceived = DateTime.Now
                }
            };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching notifications: {ErrorMessage}", ex.Message);
                ViewBag.GmailNotifications = new List<GmailNotification>
            {
                new GmailNotification
                {
                    Subject = "Error fetching notifications",
                    Sender = "System",
                    DateReceived = DateTime.Now
                }
            };
            }



            // Set default date range to the current month
            DateTime today = DateTime.Today;
            DateTime start = string.IsNullOrEmpty(startDate) ? new DateTime(today.Year, today.Month, 1) : DateTime.Parse(startDate);
            DateTime end = string.IsNullOrEmpty(endDate) ? new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)) : DateTime.Parse(endDate);

            // Fetch income and expenses based on selected date range
            decimal totalIncome = await _context.Incomes
                .Where(i => i.UserID == user.UserID && i.IncomeDate.Date >= start.Date && i.IncomeDate.Date <= end.Date)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            decimal totalExpense = await _context.Expenses
                .Where(e => e.UserID == user.UserID && e.ExpenseDate.Date >= start.Date && e.ExpenseDate.Date <= end.Date)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            decimal balance = totalIncome - totalExpense;

            //Doughnut Chart
            // Fetch expense breakdown by category
            var expenseByCategory = await _context.Expenses
                .Where(e => e.UserID == user.UserID && e.ExpenseDate.Date >= start.Date && e.ExpenseDate.Date <= end.Date)
                .GroupBy(e => e.ExpenseCategoryID)
                .Select(g => new
                {
                    categoryName = g.First().Category.Name, // Get category name
                    amount = g.Sum(e => e.Amount) // Sum expense amount
                })
                .ToListAsync();

            // Fetch user activities (Last 5 actions)
            var recentActivities = await _context.UserActivities
                .Where(a => a.UserID == user.UserID)
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .ToListAsync();

            // Fetch total expenses count for pagination
            int totalExpenses = await _context.Expenses
                .Where(e => e.UserID == user.UserID && e.ExpenseDate >= start && e.ExpenseDate <= end)
                .CountAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalExpenses / pageSize);

            // Fetch paginated recent expenses
            var recentExpenses = await _context.Expenses
                .Where(e => e.UserID == user.UserID && e.ExpenseDate >= start && e.ExpenseDate <= end)
                .OrderByDescending(e => e.ExpenseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.ExpenseID,
                    e.ExpenseDate,
                    e.Amount,
                    Category = e.Category.Name
                })
                .ToListAsync();

            //Include Expenses by Date
            var expensesByDate = await _context.Expenses
               .Where(e => e.UserID == user.UserID && e.ExpenseDate >= start && e.ExpenseDate <= end)
               .GroupBy(e => e.ExpenseDate.Date)
               .Select(g => new
               {
                  date = g.Key,
                  totalExpense = g.Sum(e => e.Amount)
               })
               .OrderBy(e => e.date)
               .ToListAsync();




            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")  // AJAX Request
            {
                return Json(new
                {
                    success = true,
                    totalIncome = totalIncome,
                    totalExpense = totalExpense,
                    balance = balance,
                    expenseByCategory = expenseByCategory,
                    recentExpenses = recentExpenses,
                    totalExpenses = totalExpenses,
                    expensesByDate = expensesByDate,  // Include expenses data
                    currentPage = page,
                    totalPages = totalPages
                });
            }

            // Pass data to View
            ViewData["TotalIncome"] = totalIncome;
            ViewData["TotalExpense"] = totalExpense;
            ViewData["Balance"] = balance;
            ViewBag.ExpenseByCategory = expenseByCategory;
            ViewBag.RecentActivities = recentActivities;
            ViewBag.RecentExpenses = recentExpenses;
            ViewBag.ExpensesByDate = expensesByDate;  // Include expenses data
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View();
        }


        public IActionResult ExportToCsv(string type)
        {
            // Retrieve the logged-in user's email from session
            string userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            int userId = user.UserID; // Filter by logged-in user's ID

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            //write header for expenses
            writer.WriteLine("Type,ID,UserID,Category,PaymentMode,Amount,Description,Date,CreatedAt");

            //Fetch and write expenses for logged-in user
            var expenses = _context.Expenses.Include(e => e.Category).Include(e => e.Payment)
                .Where(e => e.UserID == userId)
                .Select(e => new
                {
                    type = "Expense",
                    e.ExpenseID,
                    e.UserID,
                    Category = e.Category.Name,
                    PaymentMode = e.Payment.PaymentModeID,
                    e.Amount,
                    e.Description,
                    Date = e.ExpenseDate,
                    e.CreatedAt
                }).ToList();
            csv.WriteRecords(expenses);

            // Fetch and write Income for logged-in user
            var income = _context.Incomes.Include(i => i.IncomeCategory).Include(i => i.Payment)
                .Where(i => i.UserID == userId)
                .Select(i => new
                {
                    type = "Income",
                    i.IncomeID,
                    i.UserID,
                    Category = i.IncomeCategory.CategoryName,
                    PaymentMode = i.Payment.PaymentModeID,
                    i.Amount,
                    i.Description,
                    Date = i.IncomeDate,
                    i.CreatedAt
                }).ToList();
            csv.WriteRecords(income);

            //fetch and write budgets for logged-in user

            var budgets = _context.Budgets.Include(b => b.Category)
                .Where(b => b.UserID == userId)
                .Select(b => new
                {
                    type = "Budget",
                    b.BudgetID,
                    b.UserID,
                    Category = b.Category.Name,
                    PaymentMode = "", //Budget don't have a payment mode
                    b.Amount,
                    Description = "", //Budget don't have descriptions
                    Date = $"{b.Month}-{b.Year}",
                    b.CreatedAt
                }).ToList();
            csv.WriteRecords(budgets);

            writer.Flush();
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", "DashboardData.csv");
        }

        public IActionResult ExportToExcel()
        {
            // Retrieve the logged-in user's email from session
            string userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            int userId = user.UserID; // Filter by logged-in user's ID

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DashboardData");

            //Headers
            worksheet.Cells[1, 1].Value = "Type";
            worksheet.Cells[1, 2].Value = "ID";
            worksheet.Cells[1, 3].Value = "UserID";
            worksheet.Cells[1, 4].Value = "Category";
            worksheet.Cells[1, 5].Value = "PaymentMode";
            worksheet.Cells[1, 6].Value = "Amount";
            worksheet.Cells[1, 7].Value = "Description";
            worksheet.Cells[1, 8].Value = "Date";
            worksheet.Cells[1, 9].Value = "CreatedAt";

            int row = 2; //Start writing from the second row


            //Fetch and write Expenses for logged-in user
            var expenses = _context.Expenses.Include(e => e.Category).Include(e => e.Payment)
                .Where(e => e.UserID == userId)
                .Select(e => new
                {
                    type = "Expense",
                    e.ExpenseID,
                    e.UserID,
                    Category = e.Category.Name,
                    PaymentMode = e.Payment.PaymentModeID,
                    e.Amount,
                    e.Description,
                    Date = e.ExpenseDate,
                    e.CreatedAt
                }).ToList();
            foreach (var e in expenses)
            {
                worksheet.Cells[row, 1].Value = e.type;
                worksheet.Cells[row, 2].Value = e.ExpenseID;
                worksheet.Cells[row, 3].Value = e.UserID;
                worksheet.Cells[row, 4].Value = e.Category;
                worksheet.Cells[row, 5].Value = e.PaymentMode;
                worksheet.Cells[row, 6].Value = e.Amount;
                worksheet.Cells[row, 7].Value = e.Description;
                worksheet.Cells[row, 8].Value = e.Date.ToString();
                worksheet.Cells[row, 9].Value = e.CreatedAt.ToString();
                row++;
            }

            //Fetch and write Incomes for logged-in user
            var incomes = _context.Incomes.Include(i => i.IncomeCategory).Include(i => i.Payment)
                .Where(i => i.UserID == userId)
                .Select(i => new
                {
                    type = "Income",
                    i.IncomeID,
                    i.UserID,
                    Category = i.IncomeCategory.CategoryName,
                    PaymentMode = i.PaymentModeID,
                    i.Amount,
                    i.Description,
                    Date = i.IncomeDate,
                    i.CreatedAt
                }).ToList();

            foreach (var i in incomes)
            {
                worksheet.Cells[row, 1].Value = i.type;
                worksheet.Cells[row, 2].Value = i.IncomeID;
                worksheet.Cells[row, 3].Value = i.UserID;
                worksheet.Cells[row, 4].Value = i.Category;
                worksheet.Cells[row, 5].Value = i.PaymentMode;
                worksheet.Cells[row, 6].Value = i.Amount;
                worksheet.Cells[row, 7].Value = i.Description;
                worksheet.Cells[row, 8].Value = i.Date.ToString();
                worksheet.Cells[row, 9].Value = i.CreatedAt.ToString();
                row++;
            }

            //Fetch and write Budgets for logged-in user
            var budgets = _context.Budgets.Include(b => b.Category)
                .Where(b => b.UserID == userId)
                .Select(b => new
                {
                    type = "Budget",
                    b.BudgetID,
                    b.UserID,
                    Category = b.Category.Name,
                    PaymentMode = "", //Budgets don't have a payment mode
                    b.Amount,
                    Description = "", //Budgets don't have descriptions
                    Date = $"{b.Month}-{b.Year}",
                    b.CreatedAt
                }).ToList();
            foreach (var b in budgets)
            {
                worksheet.Cells[row, 1].Value = b.type;
                worksheet.Cells[row, 2].Value = b.BudgetID;
                worksheet.Cells[row, 3].Value = b.UserID;
                worksheet.Cells[row, 4].Value = b.Category;
                worksheet.Cells[row, 5].Value = b.PaymentMode;
                worksheet.Cells[row, 6].Value = b.Amount;
                worksheet.Cells[row, 7].Value = b.Description;
                worksheet.Cells[row, 8].Value = b.Date;
                worksheet.Cells[row, 9].Value = b.CreatedAt.ToString();
                row++;
            }

            // Auto-fit columns
            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DashboardData.xlsx");

        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
