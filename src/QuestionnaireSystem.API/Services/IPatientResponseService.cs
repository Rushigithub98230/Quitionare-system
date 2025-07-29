using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Services;

public interface IPatientResponseService
{
    Task<ResponseSummaryDto?> GetByAssignmentIdAsync(Guid assignmentId);
    Task<IEnumerable<ResponseSummaryDto>> GetByPatientIdAsync(Guid patientId);
    Task<ResponseSummaryDto> SaveResponsesAsync(SaveResponsesDto dto);
    Task<bool> DeleteAsync(Guid id);
} 