using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(AuthResponseDto authResponse);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    AuthResponseDto? GetUserFromToken(string token);
} 