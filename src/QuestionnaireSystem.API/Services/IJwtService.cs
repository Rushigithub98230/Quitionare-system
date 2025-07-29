using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
        string? GetRoleFromToken(string token);
    }
} 