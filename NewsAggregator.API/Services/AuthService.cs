using Microsoft.EntityFrameworkCore;
using NewsAggregator.API.Models;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request);
        Task<(bool Success, string Token, string Role, string ErrorMessage)> LoginAsync(LoginRequest request);
    }
    public class AuthService(NewsAggregatorDbContext dbContext, JwtTokenService jwtService) : IAuthService
    {
        public async Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request)
        {
            if (await dbContext.Users.AnyAsync(u => u.Email == request.Email))
                return (false, "User with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "User"
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return (true, "");
        }

        public async Task<(bool Success, string Token, string Role, string ErrorMessage)> LoginAsync(LoginRequest request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return (false, Token: "", Role: "", "Invalid credentials.");

            var token = jwtService.GenerateToken(user.Id, user.Email, user.Role);
            return (true, Token: token!, Role: user.Role!, "");
        }
    }
}