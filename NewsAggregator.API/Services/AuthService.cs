using Microsoft.EntityFrameworkCore;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services.Common;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
    public class AuthService(NewsAggregatorDbContext dbContext, JwtTokenService jwtService) : IAuthService
    {
        
        public async Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request)
        {
            try
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
            catch (Exception ex)
            {
                return (false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                    return new LoginResponse
                    {
                        Success = false,
                        Token = "",
                        UserId = Guid.Empty,
                        Role = "",
                        ErrorMessage = "Invalid credentials."
                    };


                var token = jwtService.GenerateToken(user.Id, user.Email, user.Role);
                return new LoginResponse
                {
                    Success = true,
                    Token = token,
                    UserId = user.Id,
                    Role = user.Role,
                    ErrorMessage = ""
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Token = "",
                    UserId = Guid.Empty,
                    Role = "",
                    ErrorMessage = "An unexpected error occurred: " + ex.Message
                };
            }
        }
    }
}