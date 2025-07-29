using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace QuestionnaireSystem.API.Tests
{
    public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly QuestionnaireDbContext _context;
        protected string? _adminToken;
        protected string? _userToken;

        protected TestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<QuestionnaireDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database
                    services.AddDbContext<QuestionnaireDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                    });
                });
            });

            _client = _factory.CreateClient();
            _context = _factory.Services.GetRequiredService<QuestionnaireDbContext>();
            
            // Ensure database is created
            _context.Database.EnsureCreated();
        }

        protected async Task<string> GetAdminTokenAsync()
        {
            if (_adminToken != null) return _adminToken;

            var registerData = new
            {
                firstName = "Admin",
                lastName = "User",
                email = "admin@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "Admin"
            };

            var response = await _client.PostAsync("/api/auth/register", 
                new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var result = await JsonSerializer.DeserializeAsync<AuthResponse>(await response.Content.ReadAsStreamAsync());
                _adminToken = result?.Token;
                return _adminToken!;
            }

            throw new Exception("Failed to get admin token");
        }

        protected async Task<string> GetUserTokenAsync()
        {
            if (_userToken != null) return _userToken;

            var registerData = new
            {
                firstName = "Regular",
                lastName = "User",
                email = "user@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            var response = await _client.PostAsync("/api/auth/register", 
                new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var result = await JsonSerializer.DeserializeAsync<AuthResponse>(await response.Content.ReadAsStreamAsync());
                _userToken = result?.Token;
                return _userToken!;
            }

            throw new Exception("Failed to get user token");
        }

        protected void SetAuthHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(endpoint, content);
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync(endpoint, content);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _client.DeleteAsync(endpoint);
        }

        protected async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        protected async Task<Category> CreateTestCategoryAsync(string name = "Test Category")
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Test category description",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        protected async Task<QuestionnaireTemplate> CreateTestQuestionnaireAsync(Category category)
        {
            var questionnaire = new QuestionnaireTemplate
            {
                Id = Guid.NewGuid(),
                Title = "Test Questionnaire",
                Description = "Test questionnaire description",
                CategoryId = category.Id,
                Version = "1.0",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.QuestionnaireTemplates.Add(questionnaire);
            await _context.SaveChangesAsync();
            return questionnaire;
        }

        protected async Task<Question> CreateTestQuestionAsync(QuestionnaireTemplate questionnaire, int questionTypeId = 1)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Text = "Test Question",
                QuestionTypeId = questionTypeId,
                Order = 1,
                IsRequired = true,
                QuestionnaireId = questionnaire.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        protected async Task<QuestionOption> CreateTestOptionAsync(Question question)
        {
            var option = new QuestionOption
            {
                Id = Guid.NewGuid(),
                Text = "Test Option",
                Order = 1,
                QuestionId = question.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.QuestionOptions.Add(option);
            await _context.SaveChangesAsync();
            return option;
        }

        protected class AuthResponse
        {
            public string Token { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public string ExpiresAt { get; set; } = string.Empty;
            public UserDto User { get; set; } = new();
        }

        protected class UserDto
        {
            public string Id { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
} 