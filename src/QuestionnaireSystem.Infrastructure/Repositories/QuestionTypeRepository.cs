using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class QuestionTypeRepository : IQuestionTypeRepository
{
    private readonly QuestionnaireDbContext _context;

    public QuestionTypeRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuestionType>> GetAllAsync()
    {
        return await _context.QuestionTypes
            .OrderBy(qt => qt.DisplayName)
            .ToListAsync();
    }

    public async Task<QuestionType?> GetByIdAsync(Guid id)
    {
        return await _context.QuestionTypes
            .FirstOrDefaultAsync(qt => qt.Id == id);
    }

    public async Task<IEnumerable<QuestionType>> GetActiveAsync()
    {
        return await _context.QuestionTypes
            .Where(qt => qt.IsActive)
            .OrderBy(qt => qt.DisplayName)
            .ToListAsync();
    }
} 