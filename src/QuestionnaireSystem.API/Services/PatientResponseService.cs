using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using System.Text.Json;

namespace QuestionnaireSystem.API.Services;

public class PatientResponseService : IPatientResponseService
{
    private readonly IPatientResponseRepository _responseRepository;
    private readonly IPatientAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public PatientResponseService(
        IPatientResponseRepository responseRepository,
        IPatientAssignmentRepository assignmentRepository,
        IMapper mapper)
    {
        _responseRepository = responseRepository;
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<ResponseSummaryDto?> GetByAssignmentIdAsync(Guid assignmentId)
    {
        var response = await _responseRepository.GetByAssignmentIdAsync(assignmentId);
        return _mapper.Map<ResponseSummaryDto>(response);
    }

    public async Task<IEnumerable<ResponseSummaryDto>> GetByPatientIdAsync(Guid patientId)
    {
        var responses = await _responseRepository.GetByPatientIdAsync(patientId);
        return _mapper.Map<IEnumerable<ResponseSummaryDto>>(responses);
    }

    public async Task<ResponseSummaryDto> SaveResponsesAsync(SaveResponsesDto dto)
    {
        // Get or create patient response
        var existingResponse = await _responseRepository.GetByAssignmentIdAsync(dto.AssignmentId);
        PatientResponse response;

        if (existingResponse == null)
        {
            // Get assignment to get patient and questionnaire info
            var assignment = await _assignmentRepository.GetByIdAsync(dto.AssignmentId);
            if (assignment == null)
                throw new ArgumentException("Assignment not found");

            response = new PatientResponse
            {
                AssignmentId = dto.AssignmentId,
                PatientId = assignment.PatientId,
                QuestionnaireId = assignment.QuestionnaireId,
                StartedAt = DateTime.UtcNow,
                IsDraft = dto.IsDraft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            response = await _responseRepository.CreateAsync(response);
        }
        else
        {
            response = existingResponse;
            response.IsDraft = dto.IsDraft;
            response.UpdatedAt = DateTime.UtcNow;

            if (!dto.IsDraft)
            {
                response.IsCompleted = true;
                response.CompletedAt = DateTime.UtcNow;
            }

            response = await _responseRepository.UpdateAsync(response);
        }

        // Process question responses
        foreach (var responseDto in dto.Responses)
        {
            var questionResponse = new QuestionResponse
            {
                ResponseId = response.Id,
                QuestionId = responseDto.QuestionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Set appropriate response value based on question type
            if (responseDto.TextResponse != null)
                questionResponse.TextResponse = responseDto.TextResponse;
            if (responseDto.NumberResponse.HasValue)
                questionResponse.NumberResponse = responseDto.NumberResponse.Value;
            if (responseDto.DateResponse.HasValue)
                questionResponse.DateResponse = responseDto.DateResponse.Value;
            if (responseDto.DatetimeResponse.HasValue)
                questionResponse.DatetimeResponse = responseDto.DatetimeResponse.Value;
            if (responseDto.BooleanResponse.HasValue)
                questionResponse.BooleanResponse = responseDto.BooleanResponse.Value;
            if (responseDto.JsonResponse != null)
                questionResponse.JsonResponse = JsonSerializer.Serialize(responseDto.JsonResponse);

            response.QuestionResponses.Add(questionResponse);

            // Process option responses
            foreach (var selectedOption in responseDto.SelectedOptions)
            {
                var optionResponse = new QuestionOptionResponse
                {
                    QuestionResponseId = questionResponse.Id,
                    OptionId = selectedOption.OptionId,
                    CustomText = selectedOption.CustomText,
                    CreatedAt = DateTime.UtcNow
                };

                questionResponse.OptionResponses.Add(optionResponse);
            }
        }

        // Save the updated response
        response = await _responseRepository.UpdateAsync(response);

        return _mapper.Map<ResponseSummaryDto>(response);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _responseRepository.DeleteAsync(id);
    }
} 