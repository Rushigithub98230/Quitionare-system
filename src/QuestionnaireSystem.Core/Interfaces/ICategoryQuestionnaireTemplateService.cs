using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryQuestionnaireTemplateService
{
    Task<CategoryQuestionnaireTemplateDto?> GetByIdAsync(Guid id);
    Task<CategoryQuestionnaireTemplateWithCategoryDto?> GetByIdWithCategoryAsync(Guid id);
    Task<CategoryQuestionnaireTemplateWithResponsesDto?> GetByIdWithResponsesAsync(Guid id);
    Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetAllAsync();
    Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetActiveAsync();
    Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetByUserIdAsync(Guid userId);
    Task<CategoryQuestionnaireTemplateDto> CreateAsync(CreateCategoryQuestionnaireTemplateDto createDto);
    Task<CategoryQuestionnaireTemplateDto?> UpdateAsync(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetCountByCategoryIdAsync(Guid categoryId);
    Task<bool> HasResponsesAsync(Guid questionnaireId);
} 