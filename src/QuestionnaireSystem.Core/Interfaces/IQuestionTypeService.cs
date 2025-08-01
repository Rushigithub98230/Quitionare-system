using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IQuestionTypeService
{
    Task<JsonModel> GetAllAsync(TokenModel tokenModel);
    Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> GetActiveAsync(TokenModel tokenModel);
} 