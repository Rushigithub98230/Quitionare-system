using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services;

public interface IPatientAssignmentService
{
    Task<PatientQuestionnaireAssignment?> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientQuestionnaireAssignment>> GetByPatientIdAsync(Guid patientId);
    Task<PatientQuestionnaireAssignment> AssignAsync(AssignQuestionnaireDto dto, Guid assignedBy);
    Task<PatientQuestionnaireAssignment> UpdateAsync(PatientQuestionnaireAssignment assignment);
    Task<bool> DeleteAsync(Guid id);
} 