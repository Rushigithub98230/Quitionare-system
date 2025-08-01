using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.API.Services;

public class AuthService : IAuthService
{
    private readonly QuestionnaireDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(QuestionnaireDbContext context, IJwtService jwtService, IMapper mapper)
    {
        _context = context;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<JsonModel> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return JsonModel.ErrorResult("Invalid email or password", HttpStatusCodes.Unauthorized);
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            var authResponse = new AuthResponseDto
            {
                User = userDto,
                RefreshToken = _jwtService.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60"))
            };

            authResponse.Token = _jwtService.GenerateToken(authResponse);

            return JsonModel.SuccessResult(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error during login: {ex.Message}");
        }
    }

    public async Task<JsonModel> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return JsonModel.ErrorResult("User with this email already exists", HttpStatusCodes.Conflict);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Role = registerDto.Role,
                Category = registerDto.Category,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            var authResponse = new AuthResponseDto
            {
                User = userDto,
                RefreshToken = _jwtService.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60"))
            };

            authResponse.Token = _jwtService.GenerateToken(authResponse);

            return JsonModel.SuccessResult(authResponse, "Registration successful");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error during registration: {ex.Message}");
        }
    }

    public async Task<JsonModel> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // In a real application, you would store refresh tokens in the database
            // and validate them. For now, we'll just generate a new token.
            return JsonModel.ErrorResult("Refresh token functionality not implemented", HttpStatusCodes.NotImplemented);
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error refreshing token: {ex.Message}");
        }
    }

    public async Task<JsonModel> ValidateTokenAsync(string token)
    {
        try
        {
            var isValid = _jwtService.ValidateToken(token);
            return JsonModel.SuccessResult(isValid, "Token validation completed");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error validating token: {ex.Message}");
        }
    }

    public async Task<JsonModel> GetUserProfileAsync(TokenModel tokenModel)
    {
        try
        {
            var user = await _context.Users.FindAsync(tokenModel.UserId);
            if (user == null)
                return JsonModel.NotFoundResult("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return JsonModel.SuccessResult(userDto, "User profile retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving user profile: {ex.Message}");
        }
    }

    private string HashPassword(string password)
    {
        // Temporarily using plain text for development
        return password;
    }

    private bool VerifyPassword(string password, string hash)
    {
        // Temporarily using plain text comparison for development
        return password == hash;
    }
} 