using System.ComponentModel.DataAnnotations;

namespace KeelteKoolV2.Models.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
