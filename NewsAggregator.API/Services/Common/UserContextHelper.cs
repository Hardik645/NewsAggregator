using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NewsAggregator.API.Services
{
    public static class UserContextHelper
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");
            return Guid.Parse(userIdClaim);
        }
    }
}
