using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class PatientResponseRepository : IPatientResponseRepository
{
    private readonly QuestionnaireDbContext _context;

    public PatientResponseRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<PatientResponse?> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await _context.PatientResponses
            .Include(pr => pr.QuestionResponses)
                .ThenInclude(qr => qr.OptionResponses)
            .FirstOrDefaultAsync(pr => pr.AssignmentId == assignmentId);
    }

    public async Task<PatientResponse?> GetByIdAsync(Guid id)
    {
        return await _context.PatientResponses
            .Include(pr => pr.QuestionResponses)
                .ThenInclude(qr => qr.OptionResponses)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<PatientResponse> CreateAsync(PatientResponse response)
    {
        _context.PatientResponses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<PatientResponse> UpdateAsync(PatientResponse response)
    {
        _context.PatientResponses.Update(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _context.PatientResponses.FindAsync(id);
        if (response == null) return false;

        _context.PatientResponses.Remove(response);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PatientResponse>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.PatientResponses
            .Where(pr => pr.PatientId == patientId)
            .Include(pr => pr.Questionnaire)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientResponse>> GetByQuestionnaireIdAsync(Guid questionnaireId)
    {
        return await _context.PatientResponses
            .Where(pr => pr.QuestionnaireId == questionnaireId)
            .Include(pr => pr.Questionnaire)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }
} 