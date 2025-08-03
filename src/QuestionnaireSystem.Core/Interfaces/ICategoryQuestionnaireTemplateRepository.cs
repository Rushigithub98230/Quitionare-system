using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryQuestionnaireTemplateRepository
{
    Task<CategoryQuestionnaireTemplate?> GetByIdAsync(Guid id);
    Task<CategoryQuestionnaireTemplate?> GetByIdWithQuestionsAsync(Guid id);
    Task<CategoryQuestionnaireTemplate?> GetByIdWithCategoryAsync(Guid id);
    Task<CategoryQuestionnaireTemplate?> GetByIdWithResponsesAsync(Guid id);
    Task<IEnumerable<CategoryQuestionnaireTemplate>> GetAllAsync();
    Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetSummaryAsync();
    Task<IEnumerable<CategoryQuestionnaireTemplate>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryQuestionnaireTemplate>> GetActiveAsync();
    Task<IEnumerable<CategoryQuestionnaireTemplate>> GetByUserIdAsync(Guid userId);
    Task<CategoryQuestionnaireTemplate> CreateAsync(CategoryQuestionnaireTemplate questionnaire);
    Task<CategoryQuestionnaireTemplate> UpdateAsync(CategoryQuestionnaireTemplate questionnaire);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetCountByCategoryIdAsync(Guid categoryId);
    Task<bool> HasResponsesAsync(Guid questionnaireId);
    Task<bool> TitleExistsAsync(string title, Guid? excludeId = null);
} 