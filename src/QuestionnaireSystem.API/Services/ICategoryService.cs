using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Services;

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryDto>> GetAllAsync(bool includeInactive = false);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, Guid createdBy);
    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
} 