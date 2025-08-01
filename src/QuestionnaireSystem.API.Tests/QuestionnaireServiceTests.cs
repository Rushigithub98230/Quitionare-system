using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using QuestionnaireSystem.API.Services;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;
using Xunit;
using AutoMapper;

namespace QuestionnaireSystem.API.Tests;

public class QuestionnaireServiceTests : IDisposable
{
    private readonly Mock<ICategoryQuestionnaireTemplateRepository> _mockQuestionnaireRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ICategoryQuestionRepository> _mockQuestionRepository;
    private readonly Mock<IQuestionTypeRepository> _mockQuestionTypeRepository;
    private readonly CategoryQuestionnaireTemplateService _service;
    private readonly QuestionnaireDbContext _context;

    public QuestionnaireServiceTests()
    {
        _mockQuestionnaireRepository = new Mock<ICategoryQuestionnaireTemplateRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockQuestionRepository = new Mock<ICategoryQuestionRepository>();
        _mockQuestionTypeRepository = new Mock<IQuestionTypeRepository>();

        var options = new DbContextOptionsBuilder<QuestionnaireDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new QuestionnaireDbContext(options);

        // Setup AutoMapper
        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<CategoryQuestionnaireTemplate>(It.IsAny<CreateCategoryQuestionnaireTemplateDto>()))
            .Returns((CreateCategoryQuestionnaireTemplateDto dto) => new CategoryQuestionnaireTemplate
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                IsActive = dto.IsActive,
                IsMandatory = dto.IsMandatory,
                DisplayOrder = dto.DisplayOrder,
                Version = dto.Version,
                CreatedBy = dto.CreatedBy
            });
        mockMapper.Setup(x => x.Map<CategoryQuestion>(It.IsAny<CreateCategoryQuestionDto>()))
            .Returns((CreateCategoryQuestionDto dto) => new CategoryQuestion
            {
                Id = Guid.NewGuid(),
                QuestionText = dto.QuestionText,
                QuestionTypeId = dto.QuestionTypeId,
                DisplayOrder = dto.DisplayOrder,
                IsRequired = dto.IsRequired
            });
        mockMapper.Setup(x => x.Map<CategoryQuestionnaireTemplateDto>(It.IsAny<CategoryQuestionnaireTemplate>()))
            .Returns((CategoryQuestionnaireTemplate questionnaire) => new CategoryQuestionnaireTemplateDto
            {
                Id = questionnaire.Id,
                Title = questionnaire.Title,
                Description = questionnaire.Description,
                CategoryId = questionnaire.CategoryId,
                IsActive = questionnaire.IsActive,
                IsMandatory = questionnaire.IsMandatory,
                DisplayOrder = questionnaire.DisplayOrder,
                Version = questionnaire.Version,
                CreatedBy = questionnaire.CreatedBy
            });
        mockMapper.Setup(x => x.Map(It.IsAny<UpdateCategoryQuestionnaireTemplateDto>(), It.IsAny<CategoryQuestionnaireTemplate>()))
            .Callback<UpdateCategoryQuestionnaireTemplateDto, CategoryQuestionnaireTemplate>((dto, questionnaire) =>
            {
                questionnaire.Title = dto.Title;
                questionnaire.Description = dto.Description;
                questionnaire.CategoryId = dto.CategoryId;
                questionnaire.IsActive = dto.IsActive;
                questionnaire.IsMandatory = dto.IsMandatory;
                questionnaire.DisplayOrder = dto.DisplayOrder;
                questionnaire.Version = dto.Version;
            });
        mockMapper.Setup(x => x.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(It.IsAny<IEnumerable<CategoryQuestionnaireTemplate>>()))
            .Returns((IEnumerable<CategoryQuestionnaireTemplate> questionnaires) => questionnaires.Select(q => new CategoryQuestionnaireTemplateSummaryDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                CategoryId = q.CategoryId,
                IsActive = q.IsActive,
                IsMandatory = q.IsMandatory,
                DisplayOrder = q.DisplayOrder,
                Version = q.Version,
                CreatedBy = q.CreatedBy,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
                QuestionCount = q.Questions?.Count ?? 0
            }));
        mockMapper.Setup(x => x.Map<CategoryQuestionnaireTemplateWithCategoryDto>(It.IsAny<CategoryQuestionnaireTemplate>()))
            .Returns((CategoryQuestionnaireTemplate questionnaire) => new CategoryQuestionnaireTemplateWithCategoryDto
            {
                Id = questionnaire.Id,
                Title = questionnaire.Title,
                Description = questionnaire.Description,
                CategoryId = questionnaire.CategoryId,
                IsActive = questionnaire.IsActive,
                IsMandatory = questionnaire.IsMandatory,
                DisplayOrder = questionnaire.DisplayOrder,
                Version = questionnaire.Version,
                CreatedBy = questionnaire.CreatedBy,
                Category = new CategoryDto { Id = questionnaire.CategoryId, Name = "Test Category" }
            });
        mockMapper.Setup(x => x.Map<CategoryQuestionnaireTemplateWithResponsesDto>(It.IsAny<CategoryQuestionnaireTemplate>()))
            .Returns((CategoryQuestionnaireTemplate questionnaire) => new CategoryQuestionnaireTemplateWithResponsesDto
            {
                Id = questionnaire.Id,
                Title = questionnaire.Title,
                Description = questionnaire.Description,
                CategoryId = questionnaire.CategoryId,
                IsActive = questionnaire.IsActive,
                IsMandatory = questionnaire.IsMandatory,
                DisplayOrder = questionnaire.DisplayOrder,
                Version = questionnaire.Version,
                CreatedBy = questionnaire.CreatedBy,
                UserResponses = new List<UserQuestionResponseDto>()
            });

        _service = new CategoryQuestionnaireTemplateService(
            _mockQuestionnaireRepository.Object,
            _mockQuestionRepository.Object,
            _mockCategoryRepository.Object,
            mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccessResult()
    {
        // Arrange
        var questionnaires = new List<CategoryQuestionnaireTemplateSummaryDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Test 1", CategoryId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Title = "Test 2", CategoryId = Guid.NewGuid() }
        };

        _mockQuestionnaireRepository.Setup(x => x.GetSummaryAsync())
            .ReturnsAsync(questionnaires);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnQuestionnaire()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();
        var questionnaire = new CategoryQuestionnaireTemplate
        {
            Id = questionnaireId,
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = Guid.NewGuid()
        };

        _mockQuestionnaireRepository.Setup(x => x.GetByIdWithQuestionsAsync(questionnaireId))
            .ReturnsAsync(questionnaire);

        // Act
        var result = await _service.GetByIdAsync(questionnaireId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(questionnaire.Id, result.Id);
        Assert.Equal(questionnaire.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();

        _mockQuestionnaireRepository.Setup(x => x.GetByIdWithQuestionsAsync(questionnaireId))
            .ReturnsAsync((CategoryQuestionnaireTemplate?)null);

        // Act
        var result = await _service.GetByIdAsync(questionnaireId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WithValidCategory_ShouldReturnQuestionnaire()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var questionnaires = new List<CategoryQuestionnaireTemplate>
        {
            new() { Id = Guid.NewGuid(), Title = "Test 1", CategoryId = categoryId },
            new() { Id = Guid.NewGuid(), Title = "Test 2", CategoryId = categoryId }
        };

        _mockQuestionnaireRepository.Setup(x => x.GetByCategoryIdAsync(categoryId))
            .ReturnsAsync(questionnaires);

        // Act
        var result = await _service.GetByCategoryIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, q => Assert.Equal(categoryId, q.CategoryId));
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WithInvalidCategory_ShouldReturnEmptyList()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockQuestionnaireRepository.Setup(x => x.GetByCategoryIdAsync(categoryId))
            .ReturnsAsync(new List<CategoryQuestionnaireTemplate>());

        // Act
        var result = await _service.GetByCategoryIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateQuestionnaire()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var questionTypeId = Guid.NewGuid();
        var createDto = new CreateCategoryQuestionnaireTemplateDto
        {
            Title = "Test Questionnaire",
            Description = "Test Description",
            CategoryId = categoryId,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Version = 1,
            CreatedBy = Guid.NewGuid(),
            Questions = new List<CreateCategoryQuestionDto>
            {
                new() { QuestionText = "Test Question", QuestionTypeId = questionTypeId, DisplayOrder = 1 }
            }
        };

        var category = new Category { Id = categoryId, Name = "Test Category" };
        var questionType = new QuestionType { Id = questionTypeId, TypeName = "Text" };
        var createdQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            CategoryId = categoryId
        };

        _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _mockQuestionRepository.Setup(x => x.GetQuestionTypeByIdAsync(questionTypeId))
            .ReturnsAsync(questionType);
        _mockQuestionnaireRepository.Setup(x => x.GetByCategoryIdAsync(categoryId))
            .ReturnsAsync(new List<CategoryQuestionnaireTemplate>());
        _mockQuestionnaireRepository.Setup(x => x.CreateAsync(It.IsAny<CategoryQuestionnaireTemplate>()))
            .ReturnsAsync(createdQuestionnaire);
        _mockQuestionRepository.Setup(x => x.CreateAsync(It.IsAny<CategoryQuestion>()))
            .ReturnsAsync(new CategoryQuestion());
        _mockQuestionnaireRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdQuestionnaire);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Title, result.Title);
        Assert.Equal(createDto.Description, result.Description);
        Assert.Equal(categoryId, result.CategoryId);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategory_ShouldThrowException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createDto = new CreateCategoryQuestionnaireTemplateDto
        {
            Title = "Test Questionnaire",
            CategoryId = categoryId
        };

        _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateQuestionnaire()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var updateDto = new UpdateCategoryQuestionnaireTemplateDto
        {
            Title = "Updated Questionnaire",
            Description = "Updated Description",
            CategoryId = categoryId,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 2,
            Version = 2
        };

        var existingQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = questionnaireId,
            Title = "Original Title",
            CategoryId = categoryId
        };

        var updatedQuestionnaire = new CategoryQuestionnaireTemplate
        {
            Id = questionnaireId,
            Title = updateDto.Title,
            Description = updateDto.Description,
            CategoryId = categoryId
        };

        var category = new Category { Id = categoryId, Name = "Test Category" };

        _mockQuestionnaireRepository.Setup(x => x.GetByIdAsync(questionnaireId))
            .ReturnsAsync(existingQuestionnaire);
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _mockQuestionnaireRepository.Setup(x => x.UpdateAsync(It.IsAny<CategoryQuestionnaireTemplate>()))
            .ReturnsAsync(updatedQuestionnaire);
        _mockQuestionRepository.Setup(x => x.GetByQuestionnaireIdAsync(questionnaireId))
            .ReturnsAsync(new List<CategoryQuestion>());
        _mockQuestionRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockQuestionRepository.Setup(x => x.CreateAsync(It.IsAny<CategoryQuestion>()))
            .ReturnsAsync(new CategoryQuestion());
        _mockQuestionnaireRepository.Setup(x => x.GetByIdAsync(questionnaireId))
            .ReturnsAsync(updatedQuestionnaire);

        // Act
        var result = await _service.UpdateAsync(questionnaireId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Title, result.Title);
        Assert.Equal(updateDto.Description, result.Description);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();
        var updateDto = new UpdateCategoryQuestionnaireTemplateDto
        {
            Title = "Updated Questionnaire",
            CategoryId = Guid.NewGuid()
        };

        _mockQuestionnaireRepository.Setup(x => x.GetByIdAsync(questionnaireId))
            .ReturnsAsync((CategoryQuestionnaireTemplate?)null);

        // Act
        var result = await _service.UpdateAsync(questionnaireId, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteQuestionnaire()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();

        _mockQuestionnaireRepository.Setup(x => x.DeleteAsync(questionnaireId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(questionnaireId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();

        _mockQuestionnaireRepository.Setup(x => x.DeleteAsync(questionnaireId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(questionnaireId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingResponses_ShouldReturnFalse()
    {
        // Arrange
        var questionnaireId = Guid.NewGuid();

        _mockQuestionnaireRepository.Setup(x => x.DeleteAsync(questionnaireId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(questionnaireId);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 