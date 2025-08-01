using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(AuthResponseDto authResponse)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", authResponse.User.Id.ToString()),
                    new Claim(ClaimTypes.Email, authResponse.User.Email),
                    new Claim("FirstName", authResponse.User.FirstName),
                    new Claim("LastName", authResponse.User.LastName),
                    new Claim(ClaimTypes.Role, authResponse.User.Role),
                    new Claim("Category", authResponse.User.Category ?? "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"] ?? "60")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public AuthResponseDto? GetUserFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId");
                var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var firstNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "FirstName");
                var lastNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "LastName");
                var roleClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                var categoryClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "Category");

                if (userIdClaim == null || emailClaim == null)
                    return null;

                var user = new UserDto
                {
                    Id = Guid.Parse(userIdClaim.Value),
                    Email = emailClaim.Value,
                    FirstName = firstNameClaim?.Value ?? "",
                    LastName = lastNameClaim?.Value ?? "",
                    Role = roleClaim?.Value ?? "User",
                    Category = categoryClaim?.Value
                };

                return new AuthResponseDto
                {
                    User = user,
                    Token = token
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 