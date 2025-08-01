using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class UserQuestionResponseRepository : IUserQuestionResponseRepository
{
    private readonly QuestionnaireDbContext _context;

    public UserQuestionResponseRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<UserQuestionResponse?> GetByIdAsync(Guid id)
    {
        return await _context.UserQuestionResponses
            .Include(uqr => uqr.User)
            .Include(uqr => uqr.Questionnaire)
            .Include(uqr => uqr.QuestionResponses)
            .ThenInclude(qr => qr.Question)
            .Include(uqr => uqr.QuestionResponses)
            .ThenInclude(qr => qr.OptionResponses)
            .ThenInclude(qor => qor.Option)
            .FirstOrDefaultAsync(uqr => uqr.Id == id);
    }

    public async Task<List<UserQuestionResponse>> GetByUserAsync(Guid userId)
    {
        return await _context.UserQuestionResponses
            .Include(uqr => uqr.Questionnaire)
            .Include(uqr => uqr.QuestionResponses)
            .Where(uqr => uqr.UserId == userId)
            .OrderByDescending(uqr => uqr.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserQuestionResponse>> GetByQuestionnaireAsync(Guid questionnaireId)
    {
        return await _context.UserQuestionResponses
            .Include(uqr => uqr.User)
            .Include(uqr => uqr.QuestionResponses)
            .Where(uqr => uqr.QuestionnaireId == questionnaireId)
            .OrderByDescending(uqr => uqr.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserQuestionResponse> AddAsync(UserQuestionResponse response)
    {
        _context.UserQuestionResponses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<UserQuestionResponse> CreateAsync(UserQuestionResponse response)
    {
        _context.UserQuestionResponses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public void Add(UserQuestionResponse response)
    {
        _context.UserQuestionResponses.Add(response);
    }

    public void AddQuestionResponses(List<QuestionResponse> responses)
    {
        _context.QuestionResponses.AddRange(responses);
    }

    public void AddOptionResponses(List<QuestionOptionResponse> optionResponses)
    {
        _context.QuestionOptionResponses.AddRange(optionResponses);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<QuestionResponse>> AddQuestionResponsesAsync(List<QuestionResponse> responses)
    {
        _context.QuestionResponses.AddRange(responses);
        await _context.SaveChangesAsync();
        return responses;
    }
} 