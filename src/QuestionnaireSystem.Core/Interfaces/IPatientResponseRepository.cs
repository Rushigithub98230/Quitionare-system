using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IPatientResponseRepository
{
    Task<PatientResponse?> GetByAssignmentIdAsync(Guid assignmentId);
    Task<PatientResponse?> GetByIdAsync(Guid id);
    Task<PatientResponse> CreateAsync(PatientResponse response);
    Task<PatientResponse> UpdateAsync(PatientResponse response);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PatientResponse>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<PatientResponse>> GetByQuestionnaireIdAsync(Guid questionnaireId);
} 