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
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire not found or inactive",
                    StatusCode = HttpStatusCodes.NotFound
                };

            if (tokenModel.Role != "Admin" && questionnaire.Category?.Name != tokenModel.Category)
                return new JsonModel
                {
                    Success = false,
                    Message = "Access denied to this questionnaire",
                    StatusCode = HttpStatusCodes.Forbidden
                };

            // Validate responses
            var validationResult = await ValidateResponsesAsync(dto.Responses, dto.QuestionnaireId);
            if (!validationResult.Success)
                return validationResult;

            // Create response with proper timestamp tracking
            var responseId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            
            var response = new UserQuestionResponse
            {
                Id = responseId,
                UserId = tokenModel.UserId,
                QuestionnaireId = dto.QuestionnaireId,
                StartedAt = now,
                CompletedAt = now,
                IsCompleted = true,
                IsDraft = false,
                CreatedAt = now,
                UpdatedAt = now,
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
                        DatetimeResponse = r.DateResponse, // Map date to datetime as well
                        BooleanResponse = r.TextResponse?.ToLower() switch
                        {
                            "yes" => true,
                            "no" => false,
                            _ => null
                        },
                        FilePath = r.FileUrl,
                        FileName = r.FileUrl?.Split('/').LastOrDefault(),
                        FileSize = null, // TODO: Implement file size tracking
                        FileType = r.FileUrl?.Split('.').LastOrDefault(),
                        CreatedAt = now,
                        UpdatedAt = now,
                        OptionResponses = r.SelectedOptionIds?.Select(optionId => new QuestionOptionResponse
                        {
                            Id = Guid.NewGuid(),
                            QuestionResponseId = questionResponseId,
                            OptionId = optionId,
                            CreatedAt = now
                        }).ToList() ?? new List<QuestionOptionResponse>()
                    };
                }).ToList()
            };

            await _responseRepository.AddAsync(response);

            return new JsonModel
            {
                Success = true,
                Data = new { responseId = response.Id, submittedAt = response.CompletedAt },
                Message = "Response submitted successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error submitting response: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetResponseByIdAsync(Guid responseId, TokenModel tokenModel)
    {
        try
        {
            var response = await _responseRepository.GetByIdAsync(responseId);
            if (response == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Response not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if user has access to this response
            if (response.UserId != tokenModel.UserId && tokenModel.Role != "Admin")
                return new JsonModel
                {
                    Success = false,
                    Message = "Response not found", // Return not found for security
                    StatusCode = HttpStatusCodes.NotFound
                };

            var responseDto = _mapper.Map<ResponseDetailDto>(response);
            return new JsonModel
            {
                Success = true,
                Data = responseDto,
                Message = "Response retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving response: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetResponsesByQuestionnaireAsync(Guid questionnaireId, TokenModel tokenModel)
    {
        try
        {
            // Check if user has access to this questionnaire
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            if (questionnaire.Category?.Name != tokenModel.Category && tokenModel.Role != "Admin")
                return new JsonModel
                {
                    Success = false,
                    Message = "Access denied to this questionnaire",
                    StatusCode = HttpStatusCodes.Forbidden
                };

            var responses = await _responseRepository.GetByQuestionnaireAsync(questionnaireId);
            var responseDtos = _mapper.Map<List<ResponseSummaryDto>>(responses);

            return new JsonModel
            {
                Success = true,
                Data = responseDtos,
                Message = "Responses retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetUserResponsesAsync(TokenModel tokenModel)
    {
        try
        {
            var responses = await _responseRepository.GetByUserAsync(tokenModel.UserId);
            var responseDtos = _mapper.Map<List<ResponseSummaryDto>>(responses);

            return new JsonModel
            {
                Success = true,
                Data = responseDtos,
                Message = "User responses retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving user responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetAllResponsesAsync(TokenModel tokenModel)
    {
        try
        {
            // For admin, get all responses from all questionnaires
            var responses = await _responseRepository.GetAllAsync();
            var responseDtos = _mapper.Map<List<ResponseSummaryDto>>(responses);

            return new JsonModel
            {
                Success = true,
                Data = responseDtos,
                Message = "All responses retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving all responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
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
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire not found or inactive",
                    StatusCode = HttpStatusCodes.NotFound
                };

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

                // Check if a valid response is provided
                var hasValidResponse = !string.IsNullOrEmpty(response.TextResponse) || 
                                     response.NumberResponse.HasValue || 
                                     response.DateResponse.HasValue || 
                                     !string.IsNullOrEmpty(response.FileUrl) ||
                                     (response.SelectedOptionIds != null && response.SelectedOptionIds.Any());

                // For required questions, validate that they have a valid response
                if (question.IsRequired && !hasValidResponse)
                {
                    validationErrors.Add($"Required question '{question.QuestionText}' must have a valid response");
                    continue;
                }

                if (!hasValidResponse && !question.IsRequired)
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
                return new JsonModel
                {
                    Success = false,
                    Message = string.Join("; ", validationErrors),
                    StatusCode = HttpStatusCodes.BadRequest
                };

            return new JsonModel
            {
                Success = true,
                Message = "Validation successful",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error validating responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> ExportResponsesAsync(Guid questionnaireId, TokenModel tokenModel)
    {
        try
        {
            // Check if user has access to this questionnaire
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            if (questionnaire.Category?.Name != tokenModel.Category && tokenModel.Role != "Admin")
                return new JsonModel
                {
                    Success = false,
                    Message = "Access denied to this questionnaire",
                    StatusCode = HttpStatusCodes.Forbidden
                };

            var responses = await _responseRepository.GetByQuestionnaireAsync(questionnaireId);
            
            // Generate CSV content
            var csvContent = GenerateCsvContent(responses, questionnaire);
            
            return new JsonModel
            {
                Success = true,
                Data = new { csvContent = csvContent, fileName = $"responses_{questionnaireId}.csv" },
                Message = "Responses exported successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error exporting responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    private string GenerateCsvContent(List<UserQuestionResponse> responses, CategoryQuestionnaireTemplate questionnaire)
    {
        var csvLines = new List<string>();
        
        // Header
        csvLines.Add("Response ID,User ID,Questionnaire,Submitted At,Completed,Time Taken (seconds),Question ID,Question Text,Question Type,Response,Options");
        
        foreach (var response in responses)
        {
            foreach (var questionResponse in response.QuestionResponses)
            {
                var responseValue = GetResponseValue(questionResponse);
                var options = string.Join(";", questionResponse.OptionResponses.Select(o => o.Option?.OptionText ?? "Unknown"));
                
                csvLines.Add($"\"{response.Id}\",\"{response.UserId}\",\"{questionnaire.Title}\",\"{response.CompletedAt:yyyy-MM-dd HH:mm:ss}\",\"{response.IsCompleted}\",\"{response.TimeTaken}\",\"{questionResponse.QuestionId}\",\"{questionResponse.Question?.QuestionText ?? "Unknown"}\",\"{questionResponse.Question?.QuestionType?.TypeName ?? "Unknown"}\",\"{responseValue}\",\"{options}\"");
            }
        }
        
        return string.Join("\n", csvLines);
    }

    private string GetResponseValue(QuestionResponse questionResponse)
    {
        if (!string.IsNullOrEmpty(questionResponse.TextResponse))
            return questionResponse.TextResponse;
        if (questionResponse.NumberResponse.HasValue)
            return questionResponse.NumberResponse.Value.ToString();
        if (questionResponse.DateResponse.HasValue)
            return questionResponse.DateResponse.Value.ToString("yyyy-MM-dd");
        if (questionResponse.BooleanResponse.HasValue)
            return questionResponse.BooleanResponse.Value ? "Yes" : "No";
        if (!string.IsNullOrEmpty(questionResponse.FilePath))
            return $"File: {questionResponse.FileName ?? "Unknown"}";
        
        return "No response";
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