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
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedDate == null);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Categories.AsQueryable();
        
        // Always exclude soft-deleted categories
        query = query.Where(c => c.DeletedDate == null);
        
        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
            .Include(c => c.QuestionnaireTemplate)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetDeletedAsync()
    {
        return await _context.Categories
            .IgnoreQueryFilters()
            .Where(c => c.DeletedDate != null)
            .Include(c => c.QuestionnaireTemplate)
            .OrderBy(c => c.DeletedDate)
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
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .Include(c => c.QuestionnaireTemplate)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return false;

        // Soft delete the category
        category.DeletedDate = DateTime.UtcNow;
        category.IsDeleted = true;

        // Cascade soft delete all questionnaires associated with this category
        var questionnaires = await _context.CategoryQuestionnaireTemplates
            .Where(qt => qt.CategoryId == id && qt.DeletedDate == null)
            .ToListAsync();

        foreach (var questionnaire in questionnaires)
        {
            questionnaire.DeletedDate = DateTime.UtcNow;
            questionnaire.IsDeleted = true;

            // Cascade soft delete all questions in this questionnaire
            var questions = await _context.CategoryQuestions
                .Where(q => q.QuestionnaireId == questionnaire.Id && q.DeletedDate == null)
                .ToListAsync();

            foreach (var question in questions)
            {
                question.DeletedDate = DateTime.UtcNow;
                question.IsDeleted = true;

                // Cascade soft delete all options for this question
                var options = await _context.QuestionOptions
                    .Where(o => o.QuestionId == question.Id)
                    .ToListAsync();

                foreach (var option in options)
                {
                    option.DeletedDate = DateTime.UtcNow;
                    option.IsDeleted = true;
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .Include(c => c.QuestionnaireTemplate)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return false;

        // Restore the category
        category.DeletedDate = null;
        category.IsDeleted = false;

        // Cascade restore all questionnaires associated with this category
        var questionnaires = await _context.CategoryQuestionnaireTemplates
            .IgnoreQueryFilters()
            .Where(qt => qt.CategoryId == id && qt.DeletedDate != null)
            .ToListAsync();

        foreach (var questionnaire in questionnaires)
        {
            questionnaire.DeletedDate = null;
            questionnaire.IsDeleted = false;

            // Cascade restore all questions in this questionnaire
            var questions = await _context.CategoryQuestions
                .IgnoreQueryFilters()
                .Where(q => q.QuestionnaireId == questionnaire.Id && q.DeletedDate != null)
                .ToListAsync();

            foreach (var question in questions)
            {
                question.DeletedDate = null;
                question.IsDeleted = false;

                // Cascade restore all options for this question
                var options = await _context.QuestionOptions
                    .IgnoreQueryFilters()
                    .Where(o => o.QuestionId == question.Id && o.DeletedDate != null)
                    .ToListAsync();

                foreach (var option in options)
                {
                    option.DeletedDate = null;
                    option.IsDeleted = false;
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id && c.DeletedDate == null);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, bool includeInactive = true)
    {
        var query = _context.Categories.AsQueryable();
        
        if (includeInactive)
            query = query.IgnoreQueryFilters();
        else
            query = query.Where(c => c.DeletedDate == null);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> NameExistsInactiveAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Categories
            .IgnoreQueryFilters()
            .Where(c => c.DeletedDate != null);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category == null) return false;

        category.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateAsync(Guid id)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return false;

        category.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetDeactivatedAsync()
    {
        return await _context.Categories
            .IgnoreQueryFilters()
            .Where(c => !c.IsActive && c.DeletedDate == null)
            .Include(c => c.QuestionnaireTemplate)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<int> GetMaxDisplayOrderAsync()
    {
        var maxOrder = await _context.Categories
            .Where(c => c.DeletedDate == null)
            .MaxAsync(c => (int?)c.DisplayOrder);
        
        return maxOrder ?? 0;
    }
} 