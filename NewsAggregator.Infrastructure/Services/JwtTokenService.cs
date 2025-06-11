using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NewsAggregator.Infrastructure.Services
{
    public class JwtTokenService
    {
        private readonly string _jwtSecret;
        private readonly int _jwtLifespanMinutes;

        public JwtTokenService(string jwtSecret, int jwtLifespanMinutes)
        {
            _jwtSecret = jwtSecret;
            _jwtLifespanMinutes = jwtLifespanMinutes;
        }

        public string GenerateToken(Guid userId, string email, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtLifespanMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}