using System;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.API.Models;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly NewsAggregatorDbContext _dbContext;
        private readonly JwtTokenService _jwtService;

        public AuthService(NewsAggregatorDbContext dbContext, JwtTokenService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string ErrorMessage)> SignupAsync(SignupRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                return (false, "User with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "User"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return (true, "");
        }

        public async Task<(bool Success, string Token, string Role, string ErrorMessage)> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return (false, Token: "", Role: "", "Invalid credentials.");

            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role);
            return (true, Token: token!, Role: user.Role!, "");
        }
    }
}