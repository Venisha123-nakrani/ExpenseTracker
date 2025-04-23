using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class NeedHelpController : Controller
    {
        public IActionResult Index()
        {
            return View();

        }
    }
}
