using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;

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
    Task<int> GetMaxDisplayOrderAsync(Guid questionnaireId);
    Task UpdateQuestionOptionsAsync(Guid questionId, IEnumerable<UpdateQuestionOptionDto> options);
    Task ClearQuestionOptionsAsync(Guid questionId);
    
    // Question Option management methods
    Task<QuestionOption> CreateOptionAsync(QuestionOption option);
    Task<IEnumerable<QuestionOption>> GetOptionsByQuestionIdAsync(Guid questionId);
    Task<QuestionOption?> GetOptionByIdAsync(Guid optionId);
    Task<QuestionOption> UpdateOptionAsync(QuestionOption option);
    Task<bool> DeleteOptionAsync(Guid optionId);
} 