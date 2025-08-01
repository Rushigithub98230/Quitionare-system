using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class CategoryQuestionRepository : ICategoryQuestionRepository
{
    private readonly QuestionnaireDbContext _context;

    public CategoryQuestionRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryQuestion?> GetByIdAsync(Guid id)
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedAt == null);
    }

    public async Task<IEnumerable<CategoryQuestion>> GetByQuestionnaireIdAsync(Guid questionnaireId)
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .Where(q => q.QuestionnaireId == questionnaireId && q.DeletedAt == null)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryQuestion>> GetAllAsync()
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .Where(q => q.DeletedAt == null)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<CategoryQuestion> CreateAsync(CategoryQuestion question)
    {
        question.CreatedAt = DateTime.UtcNow;
        question.UpdatedAt = DateTime.UtcNow;
        
        _context.CategoryQuestions.Add(question);
        await _context.SaveChangesAsync();
        
        return question;
    }

    public async Task<CategoryQuestion> UpdateAsync(CategoryQuestion question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        
        _context.CategoryQuestions.Update(question);
        await _context.SaveChangesAsync();
        
        return question;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var question = await GetByIdAsync(id);
        if (question == null)
            return false;

        question.DeletedAt = DateTime.UtcNow;
        question.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.CategoryQuestions
            .AnyAsync(q => q.Id == id && q.DeletedAt == null);
    }

    public async Task<bool> HasResponsesAsync(Guid questionId)
    {
        return await _context.QuestionResponses
            .AnyAsync(r => r.QuestionId == questionId);
    }

    public async Task<QuestionType?> GetQuestionTypeByIdAsync(Guid questionTypeId)
    {
        return await _context.QuestionTypes
            .FirstOrDefaultAsync(qt => qt.Id == questionTypeId);
    }
} 