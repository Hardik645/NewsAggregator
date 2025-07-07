using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.API.Models
{
    public class SignupRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = default!;
    }
}