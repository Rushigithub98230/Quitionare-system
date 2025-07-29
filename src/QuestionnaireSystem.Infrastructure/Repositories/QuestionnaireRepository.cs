using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class QuestionnaireRepository : IQuestionnaireRepository
{
    private readonly QuestionnaireDbContext _context;

    public QuestionnaireRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<QuestionnaireTemplate?> GetByIdAsync(Guid id)
    {
        return await _context.QuestionnaireTemplates
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<QuestionnaireTemplate?> GetDetailsByIdAsync(Guid id)
    {
        return await _context.QuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.Questions.OrderBy(qu => qu.DisplayOrder))
                .ThenInclude(qu => qu.QuestionType)
            .Include(q => q.Questions)
                .ThenInclude(qu => qu.Options.OrderBy(o => o.DisplayOrder))
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<QuestionnaireTemplate>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.QuestionnaireTemplates.AsQueryable();
        
        if (!includeInactive)
            query = query.Where(q => q.IsActive);

        return await query
            .Include(q => q.Category)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<QuestionnaireTemplate>> GetByCategoryAsync(Guid categoryId)
    {
        return await _context.QuestionnaireTemplates
            .Where(q => q.CategoryId == categoryId && q.IsActive)
            .Include(q => q.Category)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<QuestionnaireTemplate> CreateAsync(QuestionnaireTemplate questionnaire)
    {
        _context.QuestionnaireTemplates.Add(questionnaire);
        await _context.SaveChangesAsync();
        return questionnaire;
    }

    public async Task<QuestionnaireTemplate> UpdateAsync(QuestionnaireTemplate questionnaire)
    {
        _context.QuestionnaireTemplates.Update(questionnaire);
        await _context.SaveChangesAsync();
        return questionnaire;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var questionnaire = await GetByIdAsync(id);
        if (questionnaire == null) return false;

        questionnaire.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.QuestionnaireTemplates
            .AnyAsync(q => q.Id == id);
    }

    public async Task<int> GetNextVersionAsync(Guid questionnaireId)
    {
        var maxVersion = await _context.QuestionnaireTemplates
            .Where(q => q.Id == questionnaireId)
            .MaxAsync(q => (int?)q.Version) ?? 0;

        return maxVersion + 1;
    }
} 