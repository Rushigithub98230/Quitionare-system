using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IQuestionnaireRepository
{
    Task<QuestionnaireTemplate?> GetByIdAsync(Guid id);
    Task<QuestionnaireTemplate?> GetDetailsByIdAsync(Guid id);
    Task<IEnumerable<QuestionnaireTemplate>> GetAllAsync(bool includeInactive = false);
    Task<IEnumerable<QuestionnaireTemplate>> GetByCategoryAsync(Guid categoryId);
    Task<QuestionnaireTemplate> CreateAsync(QuestionnaireTemplate questionnaire);
    Task<QuestionnaireTemplate> UpdateAsync(QuestionnaireTemplate questionnaire);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetNextVersionAsync(Guid questionnaireId);
} 