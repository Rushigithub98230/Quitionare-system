using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IUserQuestionResponseService
{
    Task<JsonModel> SubmitResponseAsync(SubmitResponseDto dto, TokenModel tokenModel);
    Task<JsonModel> GetResponseByIdAsync(Guid responseId, TokenModel tokenModel);
    Task<JsonModel> GetResponsesByQuestionnaireAsync(Guid questionnaireId, TokenModel tokenModel);
    Task<JsonModel> GetUserResponsesAsync(TokenModel tokenModel);
    Task<JsonModel> GetAllResponsesAsync(TokenModel tokenModel);
    Task<JsonModel> ValidateResponsesAsync(SubmitResponseDto dto, TokenModel tokenModel);
    Task<JsonModel> ExportResponsesAsync(Guid questionnaireId, TokenModel tokenModel);
    
    // Analytics methods
    Task<JsonModel> GetAnalyticsSummaryAsync(TokenModel tokenModel);
    Task<JsonModel> GetAnalyticsTrendsAsync(TokenModel tokenModel);
    Task<JsonModel> GetCategoryAnalyticsAsync(TokenModel tokenModel);
    Task<JsonModel> GetQuestionnaireAnalyticsAsync(TokenModel tokenModel);
} 