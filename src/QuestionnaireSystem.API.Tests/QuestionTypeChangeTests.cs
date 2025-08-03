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

public class QuestionTypeChangeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly QuestionnaireDbContext _context;

    public QuestionTypeChangeTests(WebApplicationFactory<Program> factory)
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
    public async Task UpdateQuestion_RadioToTextArea_ShouldClearOptions()
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

        var textAreaType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "textarea",
            DisplayName = "Text Area",
            Description = "Multi-line text input",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(textAreaType);

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
            QuestionText = "What is your favorite color?",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose one option",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        // Add some options for the radio question
        var option1 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "Red",
            OptionValue = "red",
            DisplayOrder = 1,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option1);

        var option2 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "Blue",
            OptionValue = "blue",
            DisplayOrder = 2,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option2);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Please describe your favorite color:",
            QuestionTypeId = textAreaType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Provide a detailed description",
            Placeholder = "Enter your description here...",
            MinLength = 10,
            MaxLength = 500,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            Options = new List<UpdateQuestionOptionDto>() // Empty options for text area
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify options were cleared
        var remainingOptions = await _context.QuestionOptions.Where(o => o.QuestionId == question.Id).ToListAsync();
        Assert.Empty(remainingOptions);
    }

    [Fact]
    public async Task UpdateQuestion_TextAreaToRadio_ShouldCreateOptions()
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

        var textAreaType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "textarea",
            DisplayName = "Text Area",
            Description = "Multi-line text input",
            HasOptions = false,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(textAreaType);

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
            QuestionText = "Please describe your favorite color:",
            QuestionTypeId = textAreaType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Provide a detailed description",
            Placeholder = "Enter your description here...",
            MinLength = 10,
            MaxLength = 500,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "What is your favorite color?",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose one option",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            Options = new List<UpdateQuestionOptionDto>
            {
                new UpdateQuestionOptionDto
                {
                    OptionText = "Red",
                    OptionValue = "red",
                    DisplayOrder = 1,
                    IsCorrect = false,
                    IsActive = true
                },
                new UpdateQuestionOptionDto
                {
                    OptionText = "Blue",
                    OptionValue = "blue",
                    DisplayOrder = 2,
                    IsCorrect = false,
                    IsActive = true
                },
                new UpdateQuestionOptionDto
                {
                    OptionText = "Green",
                    OptionValue = "green",
                    DisplayOrder = 3,
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify options were created
        var options = await _context.QuestionOptions.Where(o => o.QuestionId == question.Id).ToListAsync();
        Assert.Equal(3, options.Count);
        Assert.Contains(options, o => o.OptionText == "Red" && o.OptionValue == "red");
        Assert.Contains(options, o => o.OptionText == "Blue" && o.OptionValue == "blue");
        Assert.Contains(options, o => o.OptionText == "Green" && o.OptionValue == "green");
    }

    [Fact]
    public async Task UpdateQuestion_SameType_ShouldPreserveOptions()
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
            QuestionText = "What is your favorite color?",
            QuestionTypeId = radioType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose one option",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        // Add existing options
        var existingOption = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "Red",
            OptionValue = "red",
            DisplayOrder = 1,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(existingOption);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "What is your favorite color? (Updated)",
            QuestionTypeId = radioType.Id, // Same type
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose one option",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            Options = new List<UpdateQuestionOptionDto>
            {
                new UpdateQuestionOptionDto
                {
                    OptionText = "Blue",
                    OptionValue = "blue",
                    DisplayOrder = 1,
                    IsCorrect = false,
                    IsActive = true
                }
            }
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify new option was added
        var options = await _context.QuestionOptions.Where(o => o.QuestionId == question.Id).ToListAsync();
        Assert.Equal(2, options.Count);
        Assert.Contains(options, o => o.OptionText == "Blue" && o.OptionValue == "blue");
    }

    [Fact]
    public async Task UpdateQuestion_CheckboxToText_ShouldClearOptions()
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

        var checkboxType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "checkbox",
            DisplayName = "Checkbox",
            Description = "Multiple choice checkbox",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(checkboxType);

        var textType = new QuestionType
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
        _context.QuestionTypes.Add(textType);

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
            QuestionText = "Select your hobbies:",
            QuestionTypeId = checkboxType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose all that apply",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        // Add options for checkbox
        var option1 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "Reading",
            OptionValue = "reading",
            DisplayOrder = 1,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option1);

        var option2 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "Gaming",
            OptionValue = "gaming",
            DisplayOrder = 2,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option2);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "What are your hobbies?",
            QuestionTypeId = textType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Enter your hobbies",
            Placeholder = "e.g., Reading, Gaming, Sports",
            MinLength = 5,
            MaxLength = 200,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            Options = new List<UpdateQuestionOptionDto>() // Empty options for text
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify options were cleared
        var remainingOptions = await _context.QuestionOptions.Where(o => o.QuestionId == question.Id).ToListAsync();
        Assert.Empty(remainingOptions);
    }

    [Fact]
    public async Task UpdateQuestion_SelectToNumber_ShouldClearOptions()
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

        var selectType = new QuestionType
        {
            Id = Guid.NewGuid(),
            TypeName = "select",
            DisplayName = "Dropdown Select",
            Description = "Single choice dropdown",
            HasOptions = true,
            SupportsFileUpload = false,
            SupportsImage = false,
            ValidationRules = null,
            IsActive = true
        };
        _context.QuestionTypes.Add(selectType);

        var numberType = new QuestionType
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
        _context.QuestionTypes.Add(numberType);

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
            QuestionText = "Select your age range:",
            QuestionTypeId = selectType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Choose your age range",
            Placeholder = null,
            MinLength = null,
            MaxLength = null,
            MinValue = null,
            MaxValue = null,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        // Add options for select
        var option1 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "18-25",
            OptionValue = "18-25",
            DisplayOrder = 1,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option1);

        var option2 = new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = question.Id,
            OptionText = "26-35",
            OptionValue = "26-35",
            DisplayOrder = 2,
            IsCorrect = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.QuestionOptions.Add(option2);

        await _context.SaveChangesAsync();

        // Act
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "What is your age?",
            QuestionTypeId = numberType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            SectionName = "Personal Info",
            HelpText = "Enter your age",
            Placeholder = "Enter age",
            MinLength = null,
            MaxLength = null,
            MinValue = 18,
            MaxValue = 100,
            ImageUrl = null,
            ImageAltText = null,
            ValidationRules = null,
            ConditionalLogic = null,
            Settings = null,
            Options = new List<UpdateQuestionOptionDto>() // Empty options for number
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify options were cleared
        var remainingOptions = await _context.QuestionOptions.Where(o => o.QuestionId == question.Id).ToListAsync();
        Assert.Empty(remainingOptions);
    }

    [Fact]
    public async Task UpdateQuestion_InvalidQuestionId_ShouldReturnNotFound()
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

        await _context.SaveChangesAsync();

        var invalidQuestionId = Guid.NewGuid();
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = Guid.NewGuid(),
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>()
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categories/{category.Id}/questionnaires/{questionnaire.Id}/questions/{invalidQuestionId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_InvalidQuestionnaireId_ShouldReturnNotFound()
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

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = Guid.NewGuid(), // Invalid questionnaire ID
            QuestionText = "Test Question",
            QuestionTypeId = Guid.NewGuid(),
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        var invalidQuestionnaireId = Guid.NewGuid();
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = Guid.NewGuid(),
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>()
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categories/{category.Id}/questionnaires/{invalidQuestionnaireId}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQuestion_InvalidQuestionTypeId_ShouldReturnBadRequest()
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
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
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
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        // Create a valid question type first
        var validQuestionType = new QuestionType
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
        _context.QuestionTypes.Add(validQuestionType);

        var question = new CategoryQuestion
        {
            Id = Guid.NewGuid(),
            QuestionnaireId = questionnaire.Id,
            QuestionText = "Test Question",
            QuestionTypeId = validQuestionType.Id,
            IsRequired = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
        };
        _context.CategoryQuestions.Add(question);

        await _context.SaveChangesAsync();

        // Debug: Verify data was saved
        var savedQuestion = await _context.CategoryQuestions.FindAsync(question.Id);
        Console.WriteLine($"Test: Saved question: {(savedQuestion != null ? "found" : "not found")}");

        var invalidQuestionTypeId = Guid.NewGuid();
        var updateDto = new UpdateCategoryQuestionDto
        {
            QuestionText = "Test Question",
            QuestionTypeId = invalidQuestionTypeId,
            IsRequired = true,
            DisplayOrder = 1,
            Options = new List<UpdateQuestionOptionDto>()
        };

        var json = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/categoryquestionnairetemplates/{questionnaire.Id}/questions/{question.Id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 
