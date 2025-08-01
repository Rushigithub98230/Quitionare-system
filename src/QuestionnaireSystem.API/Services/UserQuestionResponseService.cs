using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services;

public class UserQuestionResponseService : IUserQuestionResponseService
{
    private readonly IUserQuestionResponseRepository _responseRepository;
    private readonly ICategoryQuestionnaireTemplateRepository _questionnaireRepository;
    private readonly ICategoryQuestionRepository _questionRepository;
    private readonly IQuestionTypeRepository _questionTypeRepository;
    private readonly IMapper _mapper;

    public UserQuestionResponseService(
        IUserQuestionResponseRepository responseRepository,
        ICategoryQuestionnaireTemplateRepository questionnaireRepository,
        ICategoryQuestionRepository questionRepository,
        IQuestionTypeRepository questionTypeRepository,
        IMapper mapper)
    {
        _responseRepository = responseRepository;
        _questionnaireRepository = questionnaireRepository;
        _questionRepository = questionRepository;
        _questionTypeRepository = questionTypeRepository;
        _mapper = mapper;
    }

    public async Task<JsonModel> SubmitResponseAsync(SubmitResponseDto dto, TokenModel tokenModel)
    {
        try
        {
            // Check if questionnaire exists and user has access
            var questionnaire = await _questionnaireRepository.GetByIdAsync(dto.QuestionnaireId);
            if (questionnaire == null)
                return JsonModel.NotFoundResult("Questionnaire not found or inactive");

            if (tokenModel.Role != "Admin" && questionnaire.Category?.Name != tokenModel.Category)
                return JsonModel.ErrorResult("Access denied to this questionnaire", HttpStatusCodes.Forbidden);

            // Validate responses
            var validationResult = await ValidateResponsesAsync(dto.Responses, dto.QuestionnaireId);
            if (!validationResult.Success)
                return validationResult;

            // Create response
            var responseId = Guid.NewGuid();
            var response = new UserQuestionResponse
            {
                Id = responseId,
                UserId = tokenModel.UserId,
                QuestionnaireId = dto.QuestionnaireId,
                CompletedAt = DateTime.UtcNow,
                IsCompleted = true,
                IsDraft = false,
                QuestionResponses = dto.Responses.Select(r => 
                {
                    var questionResponseId = Guid.NewGuid();
                    return new QuestionResponse
                    {
                        Id = questionResponseId,
                        ResponseId = responseId,
                        QuestionId = r.QuestionId,
                        TextResponse = r.TextResponse,
                        NumberResponse = r.NumberResponse,
                        DateResponse = r.DateResponse,
                        FilePath = r.FileUrl,
                        OptionResponses = r.SelectedOptionIds?.Select(optionId => new QuestionOptionResponse
                        {
                            Id = Guid.NewGuid(),
                            QuestionResponseId = questionResponseId,
                            OptionId = optionId
                        }).ToList() ?? new List<QuestionOptionResponse>()
                    };
                }).ToList()
            };

            await _responseRepository.AddAsync(response);

            return JsonModel.SuccessResult(response, "Response submitted successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error submitting response: {ex.Message}", HttpStatusCodes.InternalServerError);
        }
    }

    public async Task<JsonModel> GetResponseByIdAsync(Guid responseId, TokenModel tokenModel)
    {
        try
        {
            var response = await _responseRepository.GetByIdAsync(responseId);
            if (response == null)
                return JsonModel.NotFoundResult("Response not found");

            // Check if user has access to this response
            if (response.UserId != tokenModel.UserId && tokenModel.Role != "Admin")
                return JsonModel.NotFoundResult("Response not found"); // Return not found for security

            var responseDto = _mapper.Map<ResponseDetailDto>(response);
            return JsonModel.SuccessResult(responseDto, "Response retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving response: {ex.Message}");
        }
    }

    public async Task<JsonModel> GetResponsesByQuestionnaireAsync(Guid questionnaireId, TokenModel tokenModel)
    {
        try
        {
            // Check if user has access to this questionnaire
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return JsonModel.NotFoundResult("Questionnaire template not found");

            if (questionnaire.Category?.Name != tokenModel.Category && tokenModel.Role != "Admin")
                return JsonModel.ErrorResult("Access denied to this questionnaire", HttpStatusCodes.Forbidden);

            var responses = await _responseRepository.GetByQuestionnaireAsync(questionnaireId);
            var responseDtos = _mapper.Map<List<ResponseSummaryDto>>(responses);

            return JsonModel.SuccessResult(responseDtos, "Responses retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving responses: {ex.Message}", HttpStatusCodes.InternalServerError);
        }
    }

    public async Task<JsonModel> GetUserResponsesAsync(TokenModel tokenModel)
    {
        try
        {
            var responses = await _responseRepository.GetByUserAsync(tokenModel.UserId);
            var responseDtos = _mapper.Map<List<ResponseSummaryDto>>(responses);

            return JsonModel.SuccessResult(responseDtos, "User responses retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving user responses: {ex.Message}");
        }
    }

    public async Task<JsonModel> ValidateResponsesAsync(SubmitResponseDto dto, TokenModel tokenModel)
    {
        return await ValidateResponsesAsync(dto.Responses, dto.QuestionnaireId);
    }

    public async Task<JsonModel> ValidateResponsesAsync(List<CreateQuestionResponseDto> responses, Guid questionnaireId)
    {
        try
        {
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null || !questionnaire.IsActive)
                return JsonModel.NotFoundResult("Questionnaire not found or inactive");

            var questions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);
            var requiredQuestions = questions.Where(q => q.IsRequired).ToList();
            var submittedQuestionIds = responses.Select(r => r.QuestionId).ToList();

            var validationErrors = new List<string>();

            // First, validate the responses that were submitted
            foreach (var response in responses)
            {
                var question = questions.FirstOrDefault(q => q.Id == response.QuestionId);
                if (question == null)
                {
                    validationErrors.Add($"Question {response.QuestionId} not found");
                    continue;
                }

                // Validate based on question type
                var questionType = await _questionTypeRepository.GetByIdAsync(question.QuestionTypeId);
                if (questionType == null)
                {
                    validationErrors.Add($"Question type not found for question {question.Id}");
                    continue;
                }

                // Only validate if a response is provided (for non-required questions)
                var hasResponse = !string.IsNullOrEmpty(response.TextResponse) || 
                                response.NumberResponse.HasValue || 
                                response.DateResponse.HasValue || 
                                !string.IsNullOrEmpty(response.FileUrl) ||
                                (response.SelectedOptionIds != null && response.SelectedOptionIds.Any());

                if (!hasResponse && !question.IsRequired)
                    continue; // Skip validation for non-required questions with no response

                switch (questionType.TypeName.ToLower())
                {
                    case "email":
                        if (!string.IsNullOrEmpty(response.TextResponse) && !IsValidEmail(response.TextResponse))
                            validationErrors.Add("Invalid email format");
                        break;

                    case "number":
                        if (response.NumberResponse.HasValue)
                        {
                            if (response.NumberResponse.Value < 0)
                                validationErrors.Add("Invalid number format");
                        }
                        else if (!string.IsNullOrEmpty(response.TextResponse) && !IsValidNumber(response.TextResponse))
                            validationErrors.Add("Invalid number format");
                        break;

                    case "date":
                        if (response.DateResponse.HasValue)
                        {
                            if (response.DateResponse.Value > DateTime.UtcNow)
                                validationErrors.Add("Date cannot be in the future");
                        }
                        else if (!string.IsNullOrEmpty(response.TextResponse))
                        {
                            if (!DateTime.TryParse(response.TextResponse, out var date))
                                validationErrors.Add("Invalid date format");
                            else if (date > DateTime.UtcNow)
                                validationErrors.Add("Date cannot be in the future");
                        }
                        break;

                    case "rating":
                        if (response.NumberResponse.HasValue)
                        {
                            var rating = (int)response.NumberResponse.Value;
                            if (rating < 1 || rating > 5)
                                validationErrors.Add("Rating must be between 1 and 5");
                        }
                        break;

                    case "checkbox":
                        // Check for SelectedOptionIds for checkbox questions (only if required)
                        if (question.IsRequired && (response.SelectedOptionIds == null || !response.SelectedOptionIds.Any()) && 
                            string.IsNullOrEmpty(response.TextResponse))
                            validationErrors.Add($"Checkbox question '{question.QuestionText}' requires at least one selection");
                        break;

                    case "file":
                        if (string.IsNullOrEmpty(response.FileUrl))
                            validationErrors.Add($"File upload required for question '{question.QuestionText}'");
                        break;
                }
            }

            // Then check for missing required questions
            var missingQuestions = requiredQuestions.Where(q => !submittedQuestionIds.Contains(q.Id)).ToList();
            if (missingQuestions.Any())
            {
                var missingQuestionTexts = string.Join(", ", missingQuestions.Select(q => q.QuestionText));
                validationErrors.Add($"Missing required questions: {missingQuestionTexts}");
            }

            if (validationErrors.Any())
                return JsonModel.ErrorResult(string.Join("; ", validationErrors), HttpStatusCodes.BadRequest);

            return JsonModel.SuccessResult("Validation successful");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error validating responses: {ex.Message}", HttpStatusCodes.InternalServerError);
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidNumber(string number)
    {
        return decimal.TryParse(number, out _);
    }
} 