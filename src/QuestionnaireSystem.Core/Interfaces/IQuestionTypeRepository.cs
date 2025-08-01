using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IQuestionTypeRepository
{
    Task<IEnumerable<QuestionType>> GetAllAsync();
    Task<QuestionType?> GetByIdAsync(Guid id);
    Task<IEnumerable<QuestionType>> GetActiveAsync();
} 