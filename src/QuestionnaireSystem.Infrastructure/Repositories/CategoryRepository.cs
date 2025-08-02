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
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
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
        query = query.Where(c => c.DeletedAt == null);
        
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
            .Where(c => c.DeletedAt != null)
            .Include(c => c.QuestionnaireTemplate)
            .OrderBy(c => c.DeletedAt)
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
        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        // Cascade soft delete all questionnaires associated with this category
        var questionnaires = await _context.CategoryQuestionnaireTemplates
            .Where(qt => qt.CategoryId == id && qt.DeletedAt == null)
            .ToListAsync();

        foreach (var questionnaire in questionnaires)
        {
            questionnaire.DeletedAt = DateTime.UtcNow;
            questionnaire.UpdatedAt = DateTime.UtcNow;

            // Cascade soft delete all questions in this questionnaire
            var questions = await _context.CategoryQuestions
                .Where(q => q.QuestionnaireId == questionnaire.Id && q.DeletedAt == null)
                .ToListAsync();

            foreach (var question in questions)
            {
                question.DeletedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;

                // Cascade soft delete all options for this question
                var options = await _context.QuestionOptions
                    .Where(qo => qo.QuestionId == question.Id && qo.DeletedAt == null)
                    .ToListAsync();

                foreach (var option in options)
                {
                    option.DeletedAt = DateTime.UtcNow;
                    option.UpdatedAt = DateTime.UtcNow;
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
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt != null);
        
        if (category == null) return false;

        // Restore the category
        category.DeletedAt = null;
        category.UpdatedAt = DateTime.UtcNow;

        // Cascade restore all questionnaires associated with this category
        var questionnaires = await _context.CategoryQuestionnaireTemplates
            .Where(qt => qt.CategoryId == id && qt.DeletedAt != null)
            .ToListAsync();

        foreach (var questionnaire in questionnaires)
        {
            questionnaire.DeletedAt = null;
            questionnaire.UpdatedAt = DateTime.UtcNow;

            // Cascade restore all questions in this questionnaire
            var questions = await _context.CategoryQuestions
                .Where(q => q.QuestionnaireId == questionnaire.Id && q.DeletedAt != null)
                .ToListAsync();

            foreach (var question in questions)
            {
                question.DeletedAt = null;
                question.UpdatedAt = DateTime.UtcNow;

                // Cascade restore all options for this question
                var options = await _context.QuestionOptions
                    .Where(qo => qo.QuestionId == question.Id && qo.DeletedAt != null)
                    .ToListAsync();

                foreach (var option in options)
                {
                    option.DeletedAt = null;
                    option.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

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

    public async Task<int> GetMaxDisplayOrderAsync()
    {
        var maxOrder = await _context.Categories
            .Where(c => c.DeletedAt == null)
            .MaxAsync(c => (int?)c.DisplayOrder);
        
        return maxOrder ?? 0;
    }
} 