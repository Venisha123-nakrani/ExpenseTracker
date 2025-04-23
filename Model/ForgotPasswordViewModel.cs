using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
