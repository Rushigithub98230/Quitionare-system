using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryQuestionnaireTemplateService
{
    Task<JsonModel> GetByIdAsync(Guid id);
    Task<JsonModel> GetByIdWithCategoryAsync(Guid id);
    Task<JsonModel> GetByIdWithResponsesAsync(Guid id);
    Task<JsonModel> GetAllAsync();
    Task<JsonModel> GetByCategoryIdAsync(Guid categoryId);
    Task<JsonModel> GetActiveAsync();
    Task<JsonModel> GetByUserIdAsync(Guid userId);
    Task<JsonModel> CreateAsync(CreateCategoryQuestionnaireTemplateDto createDto);
    Task<JsonModel> UpdateAsync(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto);
    Task<JsonModel> DeleteAsync(Guid id);
    Task<JsonModel> ExistsAsync(Guid id);
    Task<JsonModel> GetCountByCategoryIdAsync(Guid categoryId);
    Task<JsonModel> HasResponsesAsync(Guid questionnaireId);
    
    // Question management methods
    Task<JsonModel> GetQuestionsByQuestionnaireIdAsync(Guid questionnaireId);
    Task<JsonModel> AddQuestionAsync(Guid questionnaireId, CreateCategoryQuestionDto questionDto);
    Task<JsonModel> UpdateQuestionAsync(Guid questionnaireId, Guid questionId, UpdateCategoryQuestionDto questionDto);
    Task<JsonModel> DeleteQuestionAsync(Guid questionnaireId, Guid questionId);
    
    // Question option management methods
    Task<JsonModel> GetQuestionOptionsAsync(Guid questionnaireId, Guid questionId);
    Task<JsonModel> AddQuestionOptionAsync(Guid questionnaireId, Guid questionId, CreateQuestionOptionDto optionDto);
    Task<JsonModel> UpdateQuestionOptionAsync(Guid questionnaireId, Guid questionId, Guid optionId, UpdateQuestionOptionDto optionDto);
    Task<JsonModel> DeleteQuestionOptionAsync(Guid questionnaireId, Guid questionId, Guid optionId);
    
    // Question order management
    Task<JsonModel> UpdateQuestionOrderAsync(Guid questionnaireId, List<QuestionOrderUpdateDto> orderUpdates);
} 