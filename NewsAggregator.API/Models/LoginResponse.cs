namespace NewsAggregator.API.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public string Role { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
