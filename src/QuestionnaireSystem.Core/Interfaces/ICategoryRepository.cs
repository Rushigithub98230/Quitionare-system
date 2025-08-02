using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByNameAsync(string name);
    Task<IEnumerable<Category>> GetAllAsync(bool includeInactive = false);
    Task<IEnumerable<Category>> GetDeletedAsync();
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    Task<int> GetMaxDisplayOrderAsync();
} 