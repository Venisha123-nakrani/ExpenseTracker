using ExpenseTracker.Data;
using ExpenseTracker.Model;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CategoryReport(int? SelectedCategoryId)
        {
            var categories = _context.ExpenseCategories
                .Select(c => new SelectListItem
                {
                    Value = c.ExpenseCategoryID.ToString(),
                    Text = c.Name
                }).ToList();

            var viewModel = new CategoryReportViewModel
            {
                Categories = categories,
                Expenses = new List<ExpenseDto>() // always initialize to avoid null
            };

            if (SelectedCategoryId.HasValue)
            {
                viewModel.SelectedCategoryId = SelectedCategoryId.Value;

                var expenses = _context.Expenses
                    .Where(e => e.ExpenseCategoryID == SelectedCategoryId.Value)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToList();

                viewModel.Expenses = expenses.Select(e => new ExpenseDto
                {
                    ExpenseID = e.ExpenseID,
                    Amount = e.Amount,
                    Description = e.Description,
                    ExpenseDate = e.ExpenseDate
                }).ToList();

                viewModel.TotalAmount = viewModel.Expenses.Sum(e => e.Amount);
                viewModel.ExpenseCount = viewModel.Expenses.Count;
            }

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult ExportCategoryReportToPDF(int CategoryId, string BarChartImage, string PieChartImage)
        {
            var category = _context.ExpenseCategories.FirstOrDefault(c => c.ExpenseCategoryID == CategoryId);
            var expenses = _context.Expenses
                .Where(e => e.ExpenseCategoryID == CategoryId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();

            var totalAmount = expenses.Sum(e => e.Amount);
            var expenseCount = expenses.Count;

            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph($"Category Report for: {category?.Name}", titleFont));
                doc.Add(new Paragraph($"Total Expenses: {expenseCount}", textFont));
                doc.Add(new Paragraph($"Total Amount: {totalAmount:C}", textFont));
                doc.Add(new Paragraph(" "));

                // Table
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 2f, 4f });

                table.AddCell("Date");
                table.AddCell("Amount");
                table.AddCell("Description");

                foreach (var exp in expenses)
                {
                    table.AddCell(exp.ExpenseDate.ToShortDateString());
                    table.AddCell(exp.Amount.ToString("C"));
                    table.AddCell(exp.Description);
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));

                // Insert bar chart
                if (!string.IsNullOrEmpty(BarChartImage))
                {
                    var barImgBytes = Convert.FromBase64String(BarChartImage.Split(',')[1]);
                    var barImg = iTextSharp.text.Image.GetInstance(barImgBytes);
                    barImg.ScaleToFit(500f, 300f);
                    barImg.Alignment = Element.ALIGN_CENTER;
                    doc.Add(new Paragraph("Bar Chart", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(barImg);
                    doc.Add(new Paragraph(" "));
                }

                // Insert pie chart
                if (!string.IsNullOrEmpty(PieChartImage))
                {
                    var pieImgBytes = Convert.FromBase64String(PieChartImage.Split(',')[1]);
                    var pieImg = iTextSharp.text.Image.GetInstance(pieImgBytes);
                    pieImg.ScaleToFit(300f, 300f);
                    pieImg.Alignment = Element.ALIGN_CENTER;
                    doc.Add(new Paragraph("Pie Chart", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(pieImg);
                    doc.Add(new Paragraph(" "));
                }

                doc.Close();
                byte[] pdfBytes = stream.ToArray();

                return File(pdfBytes, "application/pdf", $"CategoryReport_{category?.Name}.pdf");
            }

        }

        [HttpGet]
        public IActionResult TopCategoriesSummary(DateTime? fromDate, DateTime? toDate)
        {
            var userIdClaim = User.FindFirstValue("UserID");
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Default to last 30 days
            DateTime from = fromDate ?? DateTime.Now.AddDays(-30);
            DateTime to = toDate ?? DateTime.Now;

            var topCategories = _context.Expenses
                .Where(e => e.UserID == userId &&
                            e.ExpenseDate >= from &&
                            e.ExpenseDate <= to)
                .GroupBy(e => e.ExpenseCategoryID)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount)
                })
                .OrderByDescending(g => g.Total)
                .ToList();

            var categoryNames = topCategories
                .Select(tc => _context.ExpenseCategories
                    .Where(c => c.ExpenseCategoryID == tc.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "Unknown")
                .ToList();

            var viewModel = new TopCategoryViewModel
            {
                CategoryNames = categoryNames,
                CategoryTotals = topCategories.Select(tc => tc.Total).ToList()
            };

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult ExportTopCategoriesSummaryPdf(DateTime fromDate, DateTime toDate, string BarChartImage, string PieChartImage)
        {
            var userIdClaim = User.FindFirstValue("UserID");
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var topCategories = _context.Expenses
                .Where(e => e.UserID == userId && e.ExpenseDate >= fromDate && e.ExpenseDate <= toDate)
                .GroupBy(e => e.ExpenseCategoryID)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount)
                })
                .OrderByDescending(g => g.Total)
                .ToList();

            var categoryNames = topCategories
                .Select(tc => _context.ExpenseCategories
                    .Where(c => c.ExpenseCategoryID == tc.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "Unknown")
                .ToList();

            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("Top Categories Summary Report", titleFont));
                doc.Add(new Paragraph($"Date Range: {fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}", textFont));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(" "));

                // Table
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 4f, 2f });
                table.AddCell("Category");
                table.AddCell("Total");

                for (int i = 0; i < categoryNames.Count; i++)
                {
                    table.AddCell(categoryNames[i]);
                    table.AddCell(topCategories[i].Total.ToString("C"));
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));

                // Add Bar Chart
                if (!string.IsNullOrEmpty(BarChartImage))
                {
                    var imageBytes = Convert.FromBase64String(BarChartImage.Split(',')[1]);
                    var chartImage = iTextSharp.text.Image.GetInstance(imageBytes);
                    chartImage.ScaleToFit(500f, 300f);
                    chartImage.Alignment = Element.ALIGN_CENTER;
                    doc.Add(new Paragraph("Bar Chart", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(chartImage);
                    doc.Add(new Paragraph(" "));
                }

                // Add Pie Chart
                if (!string.IsNullOrEmpty(PieChartImage))
                {
                    var imageBytes = Convert.FromBase64String(PieChartImage.Split(',')[1]);
                    var chartImage = iTextSharp.text.Image.GetInstance(imageBytes);
                    chartImage.ScaleToFit(300f, 300f);
                    chartImage.Alignment = Element.ALIGN_CENTER;
                    doc.Add(new Paragraph("Pie Chart", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(chartImage);
                }

                doc.Close();
                return File(stream.ToArray(), "application/pdf", "TopCategoriesSummary.pdf");
            }
        }


    }

}
