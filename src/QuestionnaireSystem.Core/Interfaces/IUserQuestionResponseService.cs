using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IUserQuestionResponseService
{
    Task<JsonModel> SubmitResponseAsync(SubmitResponseDto submitDto, TokenModel tokenModel);
    Task<JsonModel> GetUserResponsesAsync(TokenModel tokenModel);
    Task<JsonModel> GetResponseByIdAsync(Guid responseId, TokenModel tokenModel);
    Task<JsonModel> GetResponsesByQuestionnaireAsync(Guid questionnaireId, TokenModel tokenModel);
    Task<JsonModel> ValidateResponsesAsync(SubmitResponseDto dto, TokenModel tokenModel);
} 