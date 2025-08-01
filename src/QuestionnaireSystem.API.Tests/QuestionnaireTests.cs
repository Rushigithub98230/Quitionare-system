using Xunit;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.API.Services;
using AutoMapper;
using QuestionnaireSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.API.Tests;

namespace QuestionnaireSystem.API.Tests;

public class QuestionnaireTests : TestBase
{
    [Fact]
    public async Task TestApiIsWorking()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/CategoryQuestionnaireTemplates");

        // Debug output
        Console.WriteLine($"GET Response Success: {result?.Success}");
        Console.WriteLine($"GET Response StatusCode: {result?.StatusCode}");
        Console.WriteLine($"GET Response Message: {result?.Message}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task TestPostEndpointIsReachable()
    {
        // Arrange - No authentication
        var createDto = new
        {
            title = "Test",
            categoryId = "11111111-1111-1111-1111-111111111111"
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Debug output
        Console.WriteLine($"POST Response Success: {result?.Success}");
        Console.WriteLine($"POST Response StatusCode: {result?.StatusCode}");
        Console.WriteLine($"POST Response Message: {result?.Message}");

        // Assert - Should fail due to authentication, but should reach the endpoint
        Assert.NotNull(result);
        // Should get 401 or 403, not 0
        Assert.NotEqual(0, result.StatusCode);
    }

    [Fact]
    public async Task TestValidPostRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            title = "Valid Test Questionnaire",
            description = "Test questionnaire description",
            categoryId = "11111111-1111-1111-1111-111111111111",
            isActive = true,
            isMandatory = false,
            displayOrder = 10
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Debug output
        Console.WriteLine($"Valid POST Response Success: {result?.Success}");
        Console.WriteLine($"Valid POST Response StatusCode: {result?.StatusCode}");
        Console.WriteLine($"Valid POST Response Message: {result?.Message}");

        // Assert - Should either succeed or fail with a proper error, not 0
        Assert.NotNull(result);
        Assert.NotEqual(0, result.StatusCode);
    }

    [Fact]
    public async Task GetAllQuestionnaires_WithHairLossUser_ShouldReturnHairLossQuestionnaires()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/CategoryQuestionnaireTemplates");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire templates retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAllQuestionnaires_WithWeightLossUser_ShouldReturnWeightLossQuestionnaires()
    {
        // Arrange
        var token = await GetAuthTokenAsync("jane@test.com", "user123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/CategoryQuestionnaireTemplates");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire templates retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAllQuestionnaires_WithAdminToken_ShouldReturnAllQuestionnaires()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/CategoryQuestionnaireTemplates");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire templates retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionnaireById_WithValidId_ShouldReturnQuestionnaire()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        // Act
        var result = await GetAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionnaireById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        Console.WriteLine($"Token: {token}");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/CategoryQuestionnaireTemplates/00000000-0000-0000-0000-000000000000");

        // Debug output
        if (result != null)
        {
            Console.WriteLine($"Response Success: {result.Success}");
            Console.WriteLine($"Response StatusCode: {result.StatusCode}");
            Console.WriteLine($"Response Message: {result.Message}");
            Console.WriteLine($"Response Data: {result.Data}");
        }

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Questionnaire template not found", result.Message);
    }

    [Fact]
    public async Task GetQuestionnaireByCategoryId_WithValidCategory_ShouldReturnQuestionnaire()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "11111111-1111-1111-1111-111111111111"; // Hair Loss category

        // Act
        var result = await GetAsync($"/api/CategoryQuestionnaireTemplates/category/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionnaireByCategoryId_WithInvalidCategory_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "00000000-0000-0000-0000-000000000000";

        // Act
        var result = await GetAsync($"/api/CategoryQuestionnaireTemplates/category/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Questionnaire template not found for this category", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithAdminToken_ShouldCreateQuestionnaire()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "22222222-2222-2222-2222-222222222222", // Use Weight Loss category which doesn't have a questionnaire yet
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1,
                    MinLength = 2,
                    MaxLength = 50
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template created successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "11111111-1111-1111-1111-111111111111", // Use correct seeded category ID
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithInvalidCategoryId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "00000000-0000-0000-0000-000000000000", // Invalid category
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Category not found", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithExistingTemplateForCategory_ShouldReturnError()
    {
        // Arrange - Create a new category first
        var newCategory = new Category
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            Name = "Test Category",
            Description = "Test category for duplicate template test",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(newCategory);

        // Create first questionnaire for this category
        var firstQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Title = "First Questionnaire",
            Description = "First questionnaire for test category",
            CategoryId = newCategory.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.Parse("cccccccc-dddd-eeee-ffff-111111111111"), // Use the admin user ID from seeded data
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(firstQuestionnaire);
        await _context.SaveChangesAsync();



        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Second Questionnaire",
            Description = "Second questionnaire for same category",
            CategoryId = "99999999-9999-9999-9999-999999999999",
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 2,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);



        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Questionnaire template already exists for this category", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithInvalidQuestionType_ShouldReturnError()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "11111111-1111-1111-1111-111111111111", // Use correct seeded category ID
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question",
                    QuestionTypeId = "00000000-0000-0000-0000-000000000000", // Invalid question type
                    IsRequired = true,
                    DisplayOrder = 1
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid question type", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithDuplicateQuestionOrder_ShouldReturnError()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "11111111-1111-1111-1111-111111111111", // Use correct seeded category ID
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "Test Question 1",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1
                },
                new
                {
                    QuestionText = "Test Question 2",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1 // Duplicate order
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Duplicate question order", result.Message);
    }

    [Fact]
    public async Task UpdateQuestionnaire_WithAdminToken_ShouldUpdateQuestionnaire()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var updateDto = new
        {
            Title = "Updated Hair Loss Assessment",
            Description = "Updated description",
            CategoryId = "11111111-1111-1111-1111-111111111111",
            IsActive = true,
            IsMandatory = true,
            DisplayOrder = 5
        };

        // Act
        var result = await PutAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}", updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template updated successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateQuestionnaire_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "00000000-0000-0000-0000-000000000000";
        var updateDto = new
        {
            Title = "Updated Questionnaire",
            Description = "Updated description",
            CategoryId = "11111111-1111-1111-1111-111111111111",
            IsActive = true,
            IsMandatory = true,
            DisplayOrder = 5
        };

        // Act
        var result = await PutAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}", updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Questionnaire template not found", result.Message);
    }

    [Fact]
    public async Task UpdateQuestionnaire_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var updateDto = new
        {
            Title = "Updated Questionnaire",
            Description = "Updated description",
            CategoryId = "11111111-1111-1111-1111-111111111111",
            IsActive = true,
            IsMandatory = true,
            DisplayOrder = 5
        };

        // Act
        var result = await PutAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}", updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task DeleteQuestionnaire_WithAdminToken_ShouldDeleteQuestionnaire()
    {
        // Arrange - Create a questionnaire without responses for deletion test
        var testQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Title = "Test Questionnaire for Deletion",
            Description = "Test questionnaire for deletion",
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Version = 1,
            CreatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(testQuestionnaire);
        await _context.SaveChangesAsync();

        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

        // Act
        var result = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template deleted successfully", result.Message);
    }

    [Fact]
    public async Task DeleteQuestionnaire_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        // Act
        var result = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task DeleteQuestionnaire_WithExistingResponses_ShouldReturnError()
    {
        // Arrange - Create a questionnaire with responses
        var questionnaireWithResponses = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Title = "Questionnaire with Responses",
            Description = "Test questionnaire with responses",
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Use correct seeded category ID
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 15,
            Version = 1,
            CreatedBy = Guid.Parse("cccccccc-dddd-eeee-ffff-111111111111"), // Use a valid user ID
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Questions = new List<CategoryQuestion>
            {
                new CategoryQuestion
                {
                    Id = Guid.Parse("cccccccc-dddd-eeee-ffff-222222222222"),
                    QuestionText = "Test Question",
                    QuestionTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    IsRequired = true,
                    DisplayOrder = 1,
                    ValidationRules = "{}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            },
            UserResponses = new List<UserQuestionResponse>
            {
                new UserQuestionResponse
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    QuestionnaireId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    CompletedAt = DateTime.UtcNow,
                    IsCompleted = true,
                    IsDraft = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };

        // Save the questionnaire to the database
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<QuestionnaireDbContext>();
            context.CategoryQuestionnaireTemplates.Add(questionnaireWithResponses);
            await context.SaveChangesAsync();
        }

        // Act
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var result = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireWithResponses.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Cannot delete questionnaire template with existing responses", result.Message);
    }

    [Fact]
    public async Task AddQuestion_WithAdminToken_ShouldAddQuestion()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var createQuestionDto = new
        {
            QuestionText = "New Test Question",
            QuestionTypeId = "11111111-1111-1111-1111-111111111111",
            IsRequired = true,
            DisplayOrder = 10,
            MinLength = 2,
            MaxLength = 50
        };

        // Act
        var result = await PostAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions", createQuestionDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Question added successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task AddQuestion_WithDuplicateOrder_ShouldReturnError()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var createQuestionDto = new
        {
            QuestionText = "New Test Question",
            QuestionTypeId = "11111111-1111-1111-1111-111111111111",
            IsRequired = true,
            DisplayOrder = 1, // Duplicate order with existing question
            MinLength = 2,
            MaxLength = 50
        };

        // Act
        var result = await PostAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions", createQuestionDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Question with order 1 already exists", result.Message);
    }

    [Fact]
    public async Task UpdateQuestion_WithAdminToken_ShouldUpdateQuestion()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var questionId = "11111111-aaaa-bbbb-cccc-dddddddddddd";
        var updateQuestionDto = new
        {
            QuestionText = "Updated Test Question",
            QuestionTypeId = "11111111-1111-1111-1111-111111111111",
            IsRequired = false,
            DisplayOrder = 1,
            MinLength = 5,
            MaxLength = 100
        };

        // Act
        var result = await PutAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions/{questionId}", updateQuestionDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Question updated successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteQuestion_WithAdminToken_ShouldDeleteQuestion()
    {
        // Arrange - Create a question without responses for deletion test
        var testQuestion = new CategoryQuestion
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            QuestionnaireId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            QuestionText = "Test Question for Deletion",
            QuestionTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            IsRequired = true,
            DisplayOrder = 20,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(testQuestion);
        await _context.SaveChangesAsync();

        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var questionId = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee";

        // Act
        var result = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions/{questionId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Question deleted successfully", result.Message);
    }

    [Fact]
    public async Task DeleteQuestion_WithExistingResponses_ShouldReturnError()
    {
        // Arrange - Create a question with responses
        var questionWithResponses = new CategoryQuestion
        {
            Id = Guid.Parse("ffffffff-eeee-dddd-cccc-bbbbbbbbbbbb"),
            QuestionnaireId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            QuestionText = "Question with Responses",
            QuestionTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            IsRequired = true,
            DisplayOrder = 25,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(questionWithResponses);

        // Create a question response
        var questionResponse = new QuestionResponse
        {
            Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            QuestionId = questionWithResponses.Id,
            ResponseId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            TextResponse = "Test response",
            CreatedAt = DateTime.UtcNow
        };
        _context.QuestionResponses.Add(questionResponse);
        await _context.SaveChangesAsync();

        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var questionId = "ffffffff-eeee-dddd-cccc-bbbbbbbbbbbb";

        // Act
        var result = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions/{questionId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Cannot delete question with existing responses", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithEmptyTitle_ShouldReturnValidationError()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "", // Empty title
            Description = "Test questionnaire description",
            CategoryId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Title is required", result.Message);
    }

    [Fact]
    public async Task CreateQuestionnaire_WithInvalidQuestionValidation_ShouldReturnError()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Title = "Test Questionnaire",
            Description = "Test questionnaire description",
            CategoryId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", // Use correct seeded category ID
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 10,
            Questions = new[]
            {
                new
                {
                    QuestionText = "", // Invalid: empty question text
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111",
                    IsRequired = true,
                    DisplayOrder = 1
                }
            }
        };

        // Act
        var result = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("QuestionText field is required", result.Message);
    }

    [Fact]
    public async Task GetQuestionnaireForResponse_WithValidId_ShouldReturnQuestionnaire()
    {
        // Arrange
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/response");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Questionnaire template retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetQuestionnaireForResponse_WithInactiveQuestionnaire_ShouldReturnNotFound()
    {
        // Arrange - Create an inactive questionnaire
        var inactiveQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Title = "Inactive Questionnaire",
            Description = "This questionnaire is inactive",
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            IsActive = false,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(inactiveQuestionnaire);
        await _context.SaveChangesAsync();

        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync($"/api/CategoryQuestionnaireTemplates/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/response");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Questionnaire template not found or inactive", result.Message);
    }
} 
