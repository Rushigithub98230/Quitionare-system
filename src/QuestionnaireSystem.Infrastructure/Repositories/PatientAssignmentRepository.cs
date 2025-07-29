using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class PatientAssignmentRepository : IPatientAssignmentRepository
{
    private readonly QuestionnaireDbContext _context;

    public PatientAssignmentRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<PatientQuestionnaireAssignment?> GetByIdAsync(Guid id)
    {
        return await _context.PatientQuestionnaireAssignments
            .Include(pqa => pqa.Questionnaire)
            .FirstOrDefaultAsync(pqa => pqa.Id == id);
    }

    public async Task<IEnumerable<PatientQuestionnaireAssignment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.PatientQuestionnaireAssignments
            .Where(pqa => pqa.PatientId == patientId)
            .Include(pqa => pqa.Questionnaire)
            .OrderByDescending(pqa => pqa.AssignedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientQuestionnaireAssignment>> GetByQuestionnaireIdAsync(Guid questionnaireId)
    {
        return await _context.PatientQuestionnaireAssignments
            .Where(pqa => pqa.QuestionnaireId == questionnaireId)
            .Include(pqa => pqa.Questionnaire)
            .OrderByDescending(pqa => pqa.AssignedAt)
            .ToListAsync();
    }

    public async Task<PatientQuestionnaireAssignment> CreateAsync(PatientQuestionnaireAssignment assignment)
    {
        _context.PatientQuestionnaireAssignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<PatientQuestionnaireAssignment> UpdateAsync(PatientQuestionnaireAssignment assignment)
    {
        _context.PatientQuestionnaireAssignments.Update(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var assignment = await _context.PatientQuestionnaireAssignments.FindAsync(id);
        if (assignment == null) return false;

        _context.PatientQuestionnaireAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.PatientQuestionnaireAssignments
            .AnyAsync(pqa => pqa.Id == id);
    }
} 