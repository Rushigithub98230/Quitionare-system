using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Helpers;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<JsonModel>> Login(LoginDto loginDto)
    {
        return await _authService.LoginAsync(loginDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<JsonModel>> Register(RegisterDto registerDto)
    {
        return await _authService.RegisterAsync(registerDto);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<JsonModel>> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        return await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
    }

    [HttpPost("validate")]
    [Authorize]
    public async Task<ActionResult<JsonModel>> ValidateToken()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            return JsonModel.ErrorResult("Token is required");
        }

        return await _authService.ValidateTokenAsync(token);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<JsonModel>> GetUserProfile()
    {
        return await _authService.GetUserProfileAsync(TokenHelper.GetToken(HttpContext));
    }
} 