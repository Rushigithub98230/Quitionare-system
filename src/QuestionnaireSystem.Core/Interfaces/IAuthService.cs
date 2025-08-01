using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IAuthService
{
    Task<JsonModel> LoginAsync(LoginDto loginDto);
    Task<JsonModel> RegisterAsync(RegisterDto registerDto);
    Task<JsonModel> RefreshTokenAsync(string refreshToken);
    Task<JsonModel> ValidateTokenAsync(string token);
    Task<JsonModel> GetUserProfileAsync(TokenModel tokenModel);
} 