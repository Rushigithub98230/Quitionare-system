using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;
    protected readonly QuestionnaireDbContext _context;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected readonly IServiceScope _serviceScope;

    protected TestBase()
    {
        // Create a unique database name for this test
        var databaseName = $"TestDb_{Guid.NewGuid()}";
        
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the database with in-memory database for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<QuestionnaireDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<QuestionnaireDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });
            });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        _serviceScope = _factory.Services.CreateScope();
        _context = _serviceScope.ServiceProvider.GetRequiredService<QuestionnaireDbContext>();
        _context.Database.EnsureCreated();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        SeedTestData().Wait();
    }

    protected virtual async Task SeedTestData()
    {
        // Seed categories - using different GUIDs to avoid conflicts with seeded QuestionTypes
        var hairLossCategory = new Category
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Hair Loss",
            Description = "Hair loss treatment and monitoring",
            Color = "#8B4513",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var weightLossCategory = new Category
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Weight Loss",
            Description = "Weight loss and fitness tracking",
            Color = "#4CAF50",
            IsActive = true,
            DisplayOrder = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.AddRange(hairLossCategory, weightLossCategory);

        var adminUser = new User
        {
            Id = Guid.Parse("cccccccc-dddd-eeee-ffff-111111111111"),
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@test.com",
            PasswordHash = "admin123", // Plain text for development
            Role = "Admin",
            Category = "Hair Loss",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var johnUser = new User
        {
            Id = Guid.Parse("dddddddd-eeee-ffff-1111-222222222222"),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            PasswordHash = "user123", // Plain text for development
            Role = "User",
            Category = "Hair Loss",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var janeUser = new User
        {
            Id = Guid.Parse("eeeeeeee-ffff-1111-2222-333333333333"),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            PasswordHash = "user123", // Plain text for development
            Role = "User",
            Category = "Weight Loss",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.AddRange(adminUser, johnUser, janeUser);

        // Seed question types
        var questionTypes = new List<QuestionType>
        {
            new() { Id = Guid.Parse("11111111-aaaa-1111-1111-111111111111"), TypeName = "Text", DisplayName = "Text Input", HasOptions = false },
            new() { Id = Guid.Parse("22222222-bbbb-2222-2222-222222222222"), TypeName = "Textarea", DisplayName = "Long Text", HasOptions = false },
            new() { Id = Guid.Parse("33333333-cccc-3333-3333-333333333333"), TypeName = "Radio", DisplayName = "Single Choice", HasOptions = true },
            new() { Id = Guid.Parse("44444444-dddd-4444-4444-444444444444"), TypeName = "Checkbox", DisplayName = "Multiple Choice", HasOptions = true },
            new() { Id = Guid.Parse("55555555-eeee-5555-5555-555555555555"), TypeName = "Select", DisplayName = "Dropdown", HasOptions = true },
            new() { Id = Guid.Parse("66666666-ffff-6666-6666-666666666666"), TypeName = "Number", DisplayName = "Number Input", HasOptions = false },
            new() { Id = Guid.Parse("77777777-1111-7777-7777-777777777777"), TypeName = "Date", DisplayName = "Date Picker", HasOptions = false },
            new() { Id = Guid.Parse("88888888-2222-8888-8888-888888888888"), TypeName = "Email", DisplayName = "Email Input", HasOptions = false },
            new() { Id = Guid.Parse("99999999-3333-9999-9999-999999999999"), TypeName = "Phone", DisplayName = "Phone Number", HasOptions = false },
            new() { Id = Guid.Parse("aaaaaaaa-4444-aaaa-aaaa-aaaaaaaaaaaa"), TypeName = "File", DisplayName = "File Upload", HasOptions = false },
            new() { Id = Guid.Parse("bbbbbbbb-5555-bbbb-bbbb-bbbbbbbbbbbb"), TypeName = "Rating", DisplayName = "Star Rating", HasOptions = false },
            new() { Id = Guid.Parse("cccccccc-6666-cccc-cccc-cccccccccccc"), TypeName = "Slider", DisplayName = "Range Slider", HasOptions = false },
            new() { Id = Guid.Parse("dddddddd-7777-dddd-dddd-dddddddddddd"), TypeName = "Yes/No", DisplayName = "Yes/No", HasOptions = false }
        };

        _context.QuestionTypes.AddRange(questionTypes);

        // Seed questionnaire template
        var questionnaireTemplate = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            Title = "Hair Loss Assessment",
            Description = "Comprehensive hair loss assessment questionnaire",
            CategoryId = hairLossCategory.Id,
            IsActive = true,
            IsMandatory = true,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = adminUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CategoryQuestionnaireTemplates.Add(questionnaireTemplate);

        // Seed questions
        var questions = new List<CategoryQuestion>
        {
            new()
            {
                Id = Guid.Parse("11111111-aaaa-bbbb-cccc-dddddddddddd"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "What is your hair loss pattern?",
                QuestionTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Radio
                IsRequired = true,
                DisplayOrder = 1,
                ValidationRules = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("22222222-bbbb-cccc-dddd-eeeeeeeeeeee"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "How long have you been experiencing hair loss?",
                QuestionTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Text
                IsRequired = true,
                DisplayOrder = 2,
                ValidationRules = "{\"minLength\":2,\"maxLength\":100}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("33333333-cccc-dddd-eeee-ffffffffffff"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "What treatments have you tried?",
                QuestionTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // Checkbox
                IsRequired = false,
                DisplayOrder = 3,
                ValidationRules = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("44444444-dddd-eeee-ffff-111111111111"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "What is your email address?",
                QuestionTypeId = Guid.Parse("99999999-9999-9999-9999-999999999999"), // Email
                IsRequired = true,
                DisplayOrder = 4,
                ValidationRules = "{\"pattern\":\"^[\\\\w\\\\.-]+@[\\\\w\\\\.-]+\\\\.\\\\w+$\"}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("55555555-eeee-ffff-1111-222222222222"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "What is your age?",
                QuestionTypeId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // Number
                IsRequired = true,
                DisplayOrder = 5,
                ValidationRules = "{\"minValue\":18,\"maxValue\":100}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("66666666-ffff-aaaa-bbbb-cccccccccccc"),
                QuestionnaireId = questionnaireTemplate.Id,
                QuestionText = "When did you first notice hair loss?",
                QuestionTypeId = Guid.Parse("88888888-8888-8888-8888-888888888888"), // Date
                IsRequired = false,
                DisplayOrder = 6,
                ValidationRules = "{\"maxDate\":\"today\"}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.CategoryQuestions.AddRange(questions);

        // Seed question options for choice questions
        var questionOptions = new List<QuestionOption>
        {
            new()
            {
                Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                QuestionId = Guid.Parse("11111111-aaaa-bbbb-cccc-dddddddddddd"),
                OptionText = "Receding hairline",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"),
                QuestionId = Guid.Parse("11111111-aaaa-bbbb-cccc-dddddddddddd"),
                OptionText = "Crown thinning",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("cccccccc-dddd-eeee-ffff-111111111111"),
                QuestionId = Guid.Parse("11111111-aaaa-bbbb-cccc-dddddddddddd"),
                OptionText = "Overall thinning",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("dddddddd-eeee-ffff-1111-222222222222"),
                QuestionId = Guid.Parse("33333333-cccc-dddd-eeee-ffffffffffff"),
                OptionText = "Minoxidil",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("eeeeeeee-ffff-1111-2222-333333333333"),
                QuestionId = Guid.Parse("33333333-cccc-dddd-eeee-ffffffffffff"),
                OptionText = "Finasteride",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("ffffffff-1111-2222-3333-444444444444"),
                QuestionId = Guid.Parse("33333333-cccc-dddd-eeee-ffffffffffff"),
                OptionText = "Hair transplant",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.QuestionOptions.AddRange(questionOptions);

        await _context.SaveChangesAsync();
    }

    protected async Task<string> GetAuthTokenAsync(string email, string password)
    {
        var loginDto = new { Email = email, Password = password };
        var response = await PostAsync("/api/auth/login", loginDto);

        if (response?.Success == true && response.Data != null)
        {
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(
                response.Data.ToString() ?? "{}",
                _jsonOptions);

            return authResponse?.Token ?? string.Empty;
        }

        return string.Empty;
    }

    protected void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<JsonModel?> PostAsync<T>(string endpoint, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonModel>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                StatusCode = 0,
                Message = $"Request failed: {ex.Message}",
                Data = null
            };
        }
    }

    protected async Task<JsonModel?> GetAsync(string endpoint)
    {
        try
        {
            var response = await _client.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonModel>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                StatusCode = 0,
                Message = $"Request failed: {ex.Message}",
                Data = null
            };
        }
    }

    protected async Task<JsonModel?> PutAsync<T>(string endpoint, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(endpoint, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonModel>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                StatusCode = 0,
                Message = $"Request failed: {ex.Message}",
                Data = null
            };
        }
    }

    protected async Task<JsonModel?> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _client.DeleteAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonModel>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                StatusCode = 0,
                Message = $"Request failed: {ex.Message}",
                Data = null
            };
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceScope?.Dispose();
        _factory?.Dispose();
    }
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
} 