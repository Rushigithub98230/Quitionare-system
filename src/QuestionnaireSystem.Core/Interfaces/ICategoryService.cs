using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryService
{
    Task<JsonModel> GetAllAsync(TokenModel tokenModel);
    Task<JsonModel> GetDeletedAsync(TokenModel tokenModel);
    Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> CreateAsync(CreateCategoryDto dto, TokenModel tokenModel);
    Task<JsonModel> UpdateAsync(Guid id, UpdateCategoryDto dto, TokenModel tokenModel);
    Task<JsonModel> DeleteAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> RestoreAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> CheckNameExistsAsync(string name, TokenModel tokenModel);
    Task<JsonModel> DeactivateAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> ReactivateAsync(Guid id, TokenModel tokenModel);
    Task<JsonModel> GetDeactivatedAsync(TokenModel tokenModel);
} 