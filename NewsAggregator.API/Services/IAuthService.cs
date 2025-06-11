using System.Threading.Tasks;
using NewsAggregator.API.Models;

namespace NewsAggregator.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request);
        Task<(bool Success, string Token, string Role, string ErrorMessage)> LoginAsync(LoginRequest request);
    }
}