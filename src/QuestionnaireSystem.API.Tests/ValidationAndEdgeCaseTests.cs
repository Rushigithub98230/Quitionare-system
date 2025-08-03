using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.API;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace QuestionnaireSystem.API.Tests;

public class ValidationAndEdgeCaseTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly QuestionnaireDbContext _context;

    public ValidationAndEdgeCaseTests(WebApplicationFactory<Program> factory)
    {
        var databaseName = "TestDb_" + Guid.NewGuid().ToString();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<QuestionnaireDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<QuestionnaireDbContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                });
            });
        });

        _client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<QuestionnaireDbContext>();
        _context.Database.EnsureCreated();
        
        // Ensure we're using the same context for all operations
        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CreateCategory_WithLongName_ShouldReturnValidationError()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = new string('A', 101), // Exceeds 100 character limit
            Description = "Test description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test consultation description",
            Features = "Test features",
            Icon = "test-icon",
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Name must be a string or array type with a maximum length of '100'", responseContent);
    }

    [Fact]
    public async Task CreateCategory_WithLongDescription_ShouldReturnValidationError()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = new string('A', 501), // Exceeds 500 character limit
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test consultation description",
            Features = "Test features",
            Icon = "test-icon",
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Description must be a string or array type with a maximum length of '500'", responseContent);
    }

    [Fact]
    public async Task CreateCategory_WithLongConsultationDescription_ShouldReturnValidationError()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = new string('A', 501), // Exceeds 500 character limit
            Features = "Test features",
            Icon = "test-icon",
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("ConsultationDescription must be a string or array type with a maximum length of '500'", responseContent);
    }

    [Fact]
    public async Task CreateCategory_WithLongIcon_ShouldReturnValidationError()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test consultation description",
            Features = "Test features",
            Icon = new string('A', 101), // Exceeds 100 character limit
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Icon must be a string or array type with a maximum length of '100'", responseContent);
    }

    [Fact]
    public async Task CreateCategory_WithLongColor_ShouldReturnValidationError()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test description",
            Color = new string('A', 101), // Exceeds 100 character limit
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test consultation description",
            Features = "Test features",
            Icon = "test-icon",
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Color must be a string or array type with a maximum length of '100'", responseContent);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldSucceed()
    {
        // Arrange
        var categoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test consultation description",
            Features = "Test features",
            Icon = "test-icon",
            RequiresQuestionnaireAssessment = true,
            AllowsMedicationDelivery = true,
            AllowsFollowUpMessaging = true,
            IsMostPopular = true,
            IsTrending = true
        };

        // Act
        var json = JsonSerializer.Serialize(categoryDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/categories", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_WithEmptyQuestionText_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "text",
            DisplayName = "Text Input",
            Description = "Single line text input",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Original Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "", // Empty question text
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>()
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("QuestionText", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithInvalidMinMaxValues_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "number",
            DisplayName = "Number Input",
            Description = "Numeric input",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            MinValue = 100, // Min value greater than max value
            MaxValue = 50,
            Options = new List<UpdateQuestionOptionDto>()
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("MinValue cannot be greater than MaxValue", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithEmptyOptionsForRadio_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var radioType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice radio button",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(radioType);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Debug: Verify data was saved
        var savedQuestionnaire = await _context.CategoryQuestionnaireTemplates.FindAsync(questionnaire.Id);
        var savedQuestion = await _context.CategoryQuestions.FindAsync(question.Id);
        var savedQuestionType = await _context.QuestionTypes.FindAsync(radioType.Id);
        
        Console.WriteLine($"Test: Saved questionnaire: {(savedQuestionnaire != null ? "found" : "not found")}");
        Console.WriteLine($"Test: Saved question: {(savedQuestion != null ? "found" : "not found")}");
        Console.WriteLine($"Test: Saved question type: {(savedQuestionType != null ? "found" : "not found")}");

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>() // Empty options for radio type
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Radio questions must have at least one option", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithDuplicateOptionValues_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var radioType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice radio button",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(radioType);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>
            {
                new UpdateQuestionOptionDto
                {
                    OptionText = "Option 1",
                    OptionValue = "value1",
                    DisplayOrder = 1,
                    IsCorrect = false,
                    IsActive = true
                },
                new UpdateQuestionOptionDto
                {
                    OptionText = "Option 2",
                    OptionValue = "value1", // Duplicate value
                    DisplayOrder = 2,
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Duplicate option values are not allowed", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithMultipleCorrectAnswersForRadio_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var radioType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice radio button",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(radioType);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>
            {
                new UpdateQuestionOptionDto
                {
                    OptionText = "Option 1",
                    OptionValue = "value1",
                    DisplayOrder = 1,
                    IsCorrect = true, // Multiple correct answers
                    IsActive = true
                },
                new UpdateQuestionOptionDto
                {
                    OptionText = "Option 2",
                    OptionValue = "value2",
                    DisplayOrder = 2,
                    IsCorrect = true, // Multiple correct answers
                    IsActive = true
                }
            }
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Radio questions can only have one correct answer", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithLongQuestionText_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "text",
            DisplayName = "Text Input",
            Description = "Text input question",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = new string('A', 1001), // Exceeds 1000 character limit
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1
        };

        // Act
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("QuestionText cannot exceed 1000 characters", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithInvalidMinMaxLength_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "text",
            DisplayName = "Text Input",
            Description = "Text input question",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            MinLength = 10,
            MaxLength = 5 // MinLength > MaxLength
        };

        // Act
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("MinLength cannot be greater than MaxLength", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithLongOptionText_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice question",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>
            {
                new UpdateQuestionOptionDto
                {
                    OptionText = new string('A', 501), // Exceeds 500 character limit
                    OptionValue = "option1",
                    DisplayOrder = 1,
                    IsCorrect = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Option text cannot exceed 500 characters", responseContent);
    }

    [Fact]
    public async Task UpdateQuestion_WithInactiveQuestionType_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "text",
            DisplayName = "Text Input",
            Description = "Text input question",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = false // Inactive question type
        };
        _context.QuestionTypes.Add(questionType);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1
        };

        // Act
        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Only active question types can be used", responseContent);
    }

    [Fact]
    public async Task AddQuestion_WithDuplicateOptionOrders_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice question",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        await _context.SaveChangesAsync();

        var createDto = new CreateCategoryQuestionDto
        {
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question with Duplicate Orders",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<CreateQuestionOptionDto>
            {
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 1",
                    OptionValue = "option1",
                    DisplayOrder = 1,
                    IsCorrect = true,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 2",
                    OptionValue = "option2",
                    DisplayOrder = 2,
                    IsCorrect = false,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 3",
                    OptionValue = "option3",
                    DisplayOrder = 3, // Duplicate order
                    IsCorrect = false,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 4",
                    OptionValue = "option4",
                    DisplayOrder = 3, // Duplicate order
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Option display orders must be unique", responseContent);
        Assert.Contains("Found duplicate orders: 3", responseContent);
    }

    [Fact]
    public async Task AddQuestion_WithInvalidOptionOrders_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice question",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        await _context.SaveChangesAsync();

        var createDto = new CreateCategoryQuestionDto
        {
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question with Invalid Orders",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<CreateQuestionOptionDto>
            {
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 1",
                    OptionValue = "option1",
                    DisplayOrder = 1,
                    IsCorrect = true,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 2",
                    OptionValue = "option2",
                    DisplayOrder = 5, // Invalid order (should be 2)
                    IsCorrect = false,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 3",
                    OptionValue = "option3",
                    DisplayOrder = 3,
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Option display orders must be between 1 and 3", responseContent);
        Assert.Contains("Invalid orders: 5", responseContent);
    }

    [Fact]
    public async Task AddQuestion_WithMissingOptionOrders_ShouldReturnValidationError()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description",
            Color = "#007bff",
            BasePrice = 100,
            OneTimeConsultationDurationMinutes = 30,
            ConsultationDescription = "Test Consultation",
            Features = "Test Features",
            Icon = "test-icon",
            IsActive = true,
            AllowsFollowUpMessaging = true,
            AllowsMedicationDelivery = true,
            IsMostPopular = false,
            IsTrending = false,
            RequiresQuestionnaireAssessment = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = category.Id,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        var questionType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "radio",
            DisplayName = "Radio Button",
            Description = "Single choice question",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            IsActive = true
        };
        _context.QuestionTypes.Add(questionType);

        await _context.SaveChangesAsync();

        var createDto = new CreateCategoryQuestionDto
        {
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question with Missing Orders",
            QuestionTypeId = questionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<CreateQuestionOptionDto>
            {
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 1",
                    OptionValue = "option1",
                    DisplayOrder = 1,
                    IsCorrect = true,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 2",
                    OptionValue = "option2",
                    DisplayOrder = 3, // Missing order 2
                    IsCorrect = false,
                    IsActive = true
                },
                new CreateQuestionOptionDto
                {
                    OptionText = "Option 3",
                    OptionValue = "option3",
                    DisplayOrder = 0, // Missing order
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(createDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("All options must have display orders from 1 to 3", responseContent);
        Assert.Contains("Missing orders: 2", responseContent);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 
