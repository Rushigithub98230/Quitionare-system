using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Infrastructure.Data;
using QuestionnaireSystem.Infrastructure.Repositories;
using QuestionnaireSystem.API.Services;
using AutoMapper;
using QuestionnaireSystem.API.Mapping;
using Xunit;

namespace QuestionnaireSystem.API.Tests;

public class DatabaseConstraintTest : IDisposable
{
    private readonly QuestionnaireDbContext _context;
    private readonly UserQuestionResponseService _service;
    private readonly IUserQuestionResponseRepository _repository;
    private readonly ICategoryQuestionnaireTemplateRepository _questionnaireRepository;
    private readonly ICategoryQuestionRepository _questionRepository;
    private readonly IQuestionTypeRepository _questionTypeRepository;
    private readonly IMapper _mapper;

    public DatabaseConstraintTest()
    {
        // Setup in-memory database
        var services = new ServiceCollection();
        services.AddDbContext<QuestionnaireDbContext>(options =>
            options.UseInMemoryDatabase("DatabaseConstraintTest_" + Guid.NewGuid()));

        var serviceProvider = services.BuildServiceProvider();
        _context = serviceProvider.GetRequiredService<QuestionnaireDbContext>();

        // Setup repositories
        _repository = new UserQuestionResponseRepository(_context);
        _questionnaireRepository = new CategoryQuestionnaireTemplateRepository(_context);
        _questionRepository = new CategoryQuestionRepository(_context);
        _questionTypeRepository = new QuestionTypeRepository(_context);

        // Setup AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Setup service
        _service = new UserQuestionResponseService(
            _repository,
            _questionnaireRepository,
            _questionRepository,
            _questionTypeRepository,
            _mapper);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create admin user
        var adminUser = new User
        {
            Id = AdminConstants.AdminUserId,
            FirstName = AdminConstants.AdminFirstName,
            LastName = AdminConstants.AdminLastName,
            Email = "admin@test.com",
            PasswordHash = AdminConstants.AdminPassword,
            Role = AdminConstants.AdminRole,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(adminUser);

        // Create category
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Category Description",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Categories.Add(category);

        // Create questionnaire
        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.Parse("eaee95a9-e097-4853-8fd7-cfd64675dc53"),
            CategoryId = category.Id,
            Title = "Test Questionnaire",
            Description = "Test Questionnaire Description",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);

        // Create question types
        var textType = new QuestionType { Id = Guid.NewGuid(), TypeName = "text", DisplayName = "Text Input", Description = "Text Input" };
        var selectType = new QuestionType { Id = Guid.NewGuid(), TypeName = "select", DisplayName = "Select Input", Description = "Select Input" };
        var radioType = new QuestionType { Id = Guid.NewGuid(), TypeName = "radio", DisplayName = "Radio Input", Description = "Radio Input" };
        var checkboxType = new QuestionType { Id = Guid.NewGuid(), TypeName = "checkbox", DisplayName = "Checkbox Input", Description = "Checkbox Input" };
        var multiselectType = new QuestionType { Id = Guid.NewGuid(), TypeName = "multiselect", DisplayName = "Multiselect Input", Description = "Multiselect Input" };
        var dateType = new QuestionType { Id = Guid.NewGuid(), TypeName = "date", DisplayName = "Date Input", Description = "Date Input" };
        var fileType = new QuestionType { Id = Guid.NewGuid(), TypeName = "file", DisplayName = "File Input", Description = "File Input" };
        var numberType = new QuestionType { Id = Guid.NewGuid(), TypeName = "number", DisplayName = "Number Input", Description = "Number Input" };
        var emailType = new QuestionType { Id = Guid.NewGuid(), TypeName = "email", DisplayName = "Email Input", Description = "Email Input" };
        var textareaType = new QuestionType { Id = Guid.NewGuid(), TypeName = "textarea", DisplayName = "Textarea Input", Description = "Textarea Input" };

        _context.QuestionTypes.AddRange(textType, selectType, radioType, checkboxType, multiselectType, dateType, fileType, numberType, emailType, textareaType);

        // Create questions with options
        var questions = new List<CategoryQuestion>
        {
            new CategoryQuestion
            {
                Id = Guid.Parse("8622283f-ee91-4542-86db-54b204b298bc"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = textType.Id,
                QuestionText = "What is your full name?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("67abd319-dade-4bda-9353-5aaff37ebdb4"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = selectType.Id,
                QuestionText = "What is your blood type?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("beffceca-fd3f-4675-a9f4-759233c50462"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = radioType.Id,
                QuestionText = "What is your gender?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("d1e7e5cb-1d49-4657-8f99-965db8b32093"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = textareaType.Id,
                QuestionText = "Please describe your current symptoms in detail.",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("c825e6fa-7a0c-4538-a5d0-a818cdf7e786"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = checkboxType.Id,
                QuestionText = "Which of the following conditions have you been diagnosed with?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("66c81a7a-f387-4c06-b236-791e8225b585"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = multiselectType.Id,
                QuestionText = "Which languages are you comfortable speaking?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("92893113-416c-4a7f-8795-09e289db0c3f"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = dateType.Id,
                QuestionText = "What is your date of birth?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("db577ab3-5f62-4644-a27f-d5d571ed920f"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = fileType.Id,
                QuestionText = "Upload your recent medical report (PDF or image format).",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("4e4736c7-fc95-4d33-a421-b7826b7d3e67"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = numberType.Id,
                QuestionText = "What is your age?",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            },
            new CategoryQuestion
            {
                Id = Guid.Parse("c28f49e2-7839-407d-b4b2-78a9f4dbd1fa"),
                QuestionnaireId = questionnaire.Id,
                QuestionTypeId = emailType.Id,
                QuestionText = "Please enter your email address.",
                IsRequired = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.CategoryQuestions.AddRange(questions);

        // Create options for select, radio, checkbox, and multiselect questions
        var bloodTypeOptions = new List<QuestionOption>
        {
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "A+", OptionValue = "A+", DisplayOrder = 1 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "A-", OptionValue = "A-", DisplayOrder = 2 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "B+", OptionValue = "B+", DisplayOrder = 3 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "B-", OptionValue = "B-", DisplayOrder = 4 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "AB+", OptionValue = "AB+", DisplayOrder = 5 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "AB-", OptionValue = "AB-", DisplayOrder = 6 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "O+", OptionValue = "O+", DisplayOrder = 7 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[1].Id, OptionText = "O-", OptionValue = "O-", DisplayOrder = 8 }
        };

        var genderOptions = new List<QuestionOption>
        {
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[2].Id, OptionText = "Male", OptionValue = "Male", DisplayOrder = 1 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[2].Id, OptionText = "Female", OptionValue = "Female", DisplayOrder = 2 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[2].Id, OptionText = "Other", OptionValue = "Other", DisplayOrder = 3 }
        };

        var conditionOptions = new List<QuestionOption>
        {
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[4].Id, OptionText = "Diabetes", OptionValue = "Diabetes", DisplayOrder = 1 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[4].Id, OptionText = "Hypertension", OptionValue = "Hypertension", DisplayOrder = 2 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[4].Id, OptionText = "Heart Disease", OptionValue = "Heart Disease", DisplayOrder = 3 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[4].Id, OptionText = "Asthma", OptionValue = "Asthma", DisplayOrder = 4 }
        };

        var languageOptions = new List<QuestionOption>
        {
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[5].Id, OptionText = "English", OptionValue = "English", DisplayOrder = 1 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[5].Id, OptionText = "Spanish", OptionValue = "Spanish", DisplayOrder = 2 },
            new QuestionOption { Id = Guid.NewGuid(), QuestionId = questions[5].Id, OptionText = "French", OptionValue = "French", DisplayOrder = 3 }
        };

        _context.QuestionOptions.AddRange(bloodTypeOptions);
        _context.QuestionOptions.AddRange(genderOptions);
        _context.QuestionOptions.AddRange(conditionOptions);
        _context.QuestionOptions.AddRange(languageOptions);

        _context.SaveChanges();
    }

    [Fact]
    public async Task Test_DatabaseConstraints_WithNullValues_ShouldHandleGracefully()
    {
        // Arrange - Test with null values that might cause constraint violations
        var tokenModel = new TokenModel
        {
            UserId = AdminConstants.AdminUserId,
            Role = "Admin",
            Category = "Default"
        };

        var submitDto = new SubmitResponseDto
        {
            QuestionnaireId = Guid.Parse("eaee95a9-e097-4853-8fd7-cfd64675dc53"),
            Responses = new List<CreateQuestionResponseDto>
            {
                new CreateQuestionResponseDto
                {
                    QuestionId = Guid.Parse("8622283f-ee91-4542-86db-54b204b298bc"),
                    TextResponse = null,
                    NumberResponse = null,
                    DateResponse = null,
                    SelectedOptionIds = null,
                    FileUrl = null,
                    ImageUrl = null
                }
            }
        };

        // Act
        var result = await _service.SubmitResponseAsync(submitDto, tokenModel);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCodes.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Test_DatabaseConstraints_WithEmptyStrings_ShouldHandleGracefully()
    {
        // Arrange - Test with empty strings
        var tokenModel = new TokenModel
        {
            UserId = AdminConstants.AdminUserId,
            Role = "Admin",
            Category = "Default"
        };

        var submitDto = new SubmitResponseDto
        {
            QuestionnaireId = Guid.Parse("eaee95a9-e097-4853-8fd7-cfd64675dc53"),
            Responses = new List<CreateQuestionResponseDto>
            {
                new CreateQuestionResponseDto
                {
                    QuestionId = Guid.Parse("8622283f-ee91-4542-86db-54b204b298bc"),
                    TextResponse = "",
                    NumberResponse = null,
                    DateResponse = null,
                    SelectedOptionIds = new List<Guid>(),
                    FileUrl = "",
                    ImageUrl = ""
                }
            }
        };

        // Act
        var result = await _service.SubmitResponseAsync(submitDto, tokenModel);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCodes.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Test_DatabaseConstraints_WithInvalidGuid_ShouldFail()
    {
        // Arrange - Test with invalid GUID
        var tokenModel = new TokenModel
        {
            UserId = AdminConstants.AdminUserId,
            Role = "Admin",
            Category = "Default"
        };

        var submitDto = new SubmitResponseDto
        {
            QuestionnaireId = Guid.Parse("eaee95a9-e097-4853-8fd7-cfd64675dc53"),
            Responses = new List<CreateQuestionResponseDto>
            {
                new CreateQuestionResponseDto
                {
                    QuestionId = Guid.Empty, // Invalid GUID
                    TextResponse = "test",
                    SelectedOptionIds = new List<Guid>()
                }
            }
        };

        // Act
        var result = await _service.SubmitResponseAsync(submitDto, tokenModel);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCodes.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Test_DatabaseConstraints_WithVeryLongText_ShouldHandleGracefully()
    {
        // Arrange - Test with very long text that might exceed database limits
        var tokenModel = new TokenModel
        {
            UserId = AdminConstants.AdminUserId,
            Role = "Admin",
            Category = "Default"
        };

        var longText = new string('a', 10000); // Very long text

        var submitDto = new SubmitResponseDto
        {
            QuestionnaireId = Guid.Parse("eaee95a9-e097-4853-8fd7-cfd64675dc53"),
            Responses = new List<CreateQuestionResponseDto>
            {
                new CreateQuestionResponseDto
                {
                    QuestionId = Guid.Parse("8622283f-ee91-4542-86db-54b204b298bc"),
                    TextResponse = longText,
                    SelectedOptionIds = new List<Guid>()
                }
            }
        };

        // Act
        var result = await _service.SubmitResponseAsync(submitDto, tokenModel);

        // Assert
        Assert.True(result.Success, $"Response submission failed: {result.Message}");
        Assert.Equal(HttpStatusCodes.OK, result.StatusCode);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 