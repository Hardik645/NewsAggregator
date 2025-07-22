using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.API.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
    }
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public string Role { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}