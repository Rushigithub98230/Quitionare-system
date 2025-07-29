using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Services;

public interface IQuestionnaireService
{
    Task<QuestionnaireDetailDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<QuestionnaireDetailDto>> GetAllAsync(bool includeInactive = false);
    Task<IEnumerable<QuestionnaireDetailDto>> GetByCategoryAsync(Guid categoryId);
    Task<QuestionnaireDetailDto> CreateAsync(CreateQuestionnaireDto dto, Guid createdBy);
    Task<QuestionnaireDetailDto> UpdateAsync(Guid id, UpdateQuestionnaireDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<QuestionnaireResponseDto?> GetForResponseAsync(Guid id);
} 