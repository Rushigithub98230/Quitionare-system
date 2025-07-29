using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services;

public class PatientAssignmentService : IPatientAssignmentService
{
    private readonly IPatientAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public PatientAssignmentService(IPatientAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<PatientQuestionnaireAssignment?> GetByIdAsync(Guid id)
    {
        return await _assignmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PatientQuestionnaireAssignment>> GetByPatientIdAsync(Guid patientId)
    {
        return await _assignmentRepository.GetByPatientIdAsync(patientId);
    }

    public async Task<PatientQuestionnaireAssignment> AssignAsync(AssignQuestionnaireDto dto, Guid assignedBy)
    {
        var assignment = new PatientQuestionnaireAssignment
        {
            PatientId = dto.PatientId,
            QuestionnaireId = dto.QuestionnaireId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        return await _assignmentRepository.CreateAsync(assignment);
    }

    public async Task<PatientQuestionnaireAssignment> UpdateAsync(PatientQuestionnaireAssignment assignment)
    {
        return await _assignmentRepository.UpdateAsync(assignment);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _assignmentRepository.DeleteAsync(id);
    }
} 