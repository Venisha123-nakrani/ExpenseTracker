using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ChatBotController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public ChatBotController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            string apiKey = _config["OpenAI:ApiKey"];

            // ✅ Get UserID from claims
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Get total expenses for that user
            var totalExpenses = _context.Expenses
                .Where(e => e.UserID == userId)
                .Sum(e => e.Amount);

            string prompt = $"You are a smart assistant for an expense tracker. Help the user with modules like Dashboard, Income, Expenses, Recurring Expenses, and Reports. Here's their query: {request.Message} The user has spent a total of {totalExpenses} this month.";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            var botReply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { reply = botReply });

        }


        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}
