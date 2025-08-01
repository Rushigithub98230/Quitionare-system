using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryQuestionRepository
{
    Task<CategoryQuestion?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryQuestion>> GetByQuestionnaireIdAsync(Guid questionnaireId);
    Task<CategoryQuestion> CreateAsync(CategoryQuestion question);
    Task<CategoryQuestion> UpdateAsync(CategoryQuestion question);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasResponsesAsync(Guid questionId);
    Task<QuestionType?> GetQuestionTypeByIdAsync(Guid questionTypeId);
} 