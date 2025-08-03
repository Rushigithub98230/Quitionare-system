using System.Security.Claims;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Helpers;

public static class TokenHelper
{
    public static TokenModel GetToken(HttpContext httpContext)
    {
        var user = httpContext.User;
        
        var userIdClaim = user.FindFirst("UserId")?.Value;
        int userId;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out userId))
        {
            userId = 0;
        }
        
        return new TokenModel
        {
            UserId = userId,
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            Category = user.FindFirst("Category")?.Value ?? string.Empty,
            FirstName = user.FindFirst("FirstName")?.Value ?? string.Empty,
            LastName = user.FindFirst("LastName")?.Value ?? string.Empty
        };
    }
} 