using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IUserQuestionResponseRepository
{
    Task<UserQuestionResponse?> GetByIdAsync(Guid id);
    Task<List<UserQuestionResponse>> GetByUserAsync(Guid userId);
    Task<List<UserQuestionResponse>> GetByQuestionnaireAsync(Guid questionnaireId);
    Task<List<UserQuestionResponse>> GetAllAsync();
    Task<UserQuestionResponse> AddAsync(UserQuestionResponse response);
    void Add(UserQuestionResponse response);
    void AddQuestionResponses(List<QuestionResponse> responses);
    void AddOptionResponses(List<QuestionOptionResponse> optionResponses);
    Task SaveChangesAsync();
    Task<List<QuestionResponse>> AddQuestionResponsesAsync(List<QuestionResponse> responses);
    Task<List<QuestionResponse>> GetByResponseIdAsync(Guid responseId);
} 