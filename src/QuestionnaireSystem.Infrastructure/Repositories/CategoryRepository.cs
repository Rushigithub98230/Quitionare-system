using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly QuestionnaireDbContext _context;

    public CategoryRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .Include(c => c.QuestionnaireTemplate)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Categories.AsQueryable();
        
        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
            .Include(c => c.QuestionnaireTemplate)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category == null) return false;

        category.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Categories.AsQueryable();
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query
            .AnyAsync(c => c.Name == name);
    }
} 