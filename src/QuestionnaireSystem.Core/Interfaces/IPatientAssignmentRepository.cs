using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Core.Interfaces;

public interface IPatientAssignmentRepository
{
    Task<PatientQuestionnaireAssignment?> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientQuestionnaireAssignment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<PatientQuestionnaireAssignment>> GetByQuestionnaireIdAsync(Guid questionnaireId);
    Task<PatientQuestionnaireAssignment> CreateAsync(PatientQuestionnaireAssignment assignment);
    Task<PatientQuestionnaireAssignment> UpdateAsync(PatientQuestionnaireAssignment assignment);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
} 