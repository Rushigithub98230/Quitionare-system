using AutoMapper;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Services;

public class CategoryQuestionnaireTemplateService : ICategoryQuestionnaireTemplateService
{
    private readonly ICategoryQuestionnaireTemplateRepository _questionnaireRepository;
    private readonly ICategoryQuestionRepository _questionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryQuestionnaireTemplateService(
        ICategoryQuestionnaireTemplateRepository questionnaireRepository,
        ICategoryQuestionRepository questionRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _questionnaireRepository = questionnaireRepository;
        _questionRepository = questionRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<JsonModel> GetByIdAsync(Guid id)
    {
        try
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(id);
        if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var dto = _mapper.Map<CategoryQuestionnaireTemplateDto>(questionnaire);
            return new JsonModel
            {
                Success = true,
                Data = dto,
                Message = "Questionnaire template retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByIdWithCategoryAsync(Guid id)
    {
        try
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithCategoryAsync(id);
        if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var dto = _mapper.Map<CategoryQuestionnaireTemplateWithCategoryDto>(questionnaire);
            return new JsonModel
            {
                Success = true,
                Data = dto,
                Message = "Questionnaire with category retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByIdWithResponsesAsync(Guid id)
    {
        try
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithResponsesAsync(id);
        if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var dto = _mapper.Map<CategoryQuestionnaireTemplateWithResponsesDto>(questionnaire);
            return new JsonModel
            {
                Success = true,
                Data = dto,
                Message = "Questionnaire with responses retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetAllAsync()
    {
        try
    {
        var questionnaires = await _questionnaireRepository.GetSummaryAsync();
            return new JsonModel
            {
                Success = true,
                Data = questionnaires,
                Message = "Questionnaire templates retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving questionnaires: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByCategoryIdAsync(Guid categoryId)
    {
        try
    {
        var questionnaires = await _questionnaireRepository.GetByCategoryIdAsync(categoryId);
            var dtos = _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
            
            if (!dtos.Any())
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found for this category",
                    StatusCode = HttpStatusCodes.NotFound
                };
            
            return new JsonModel
            {
                Success = true,
                Data = dtos,
                Message = "Questionnaire template retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving questionnaires by category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetActiveAsync()
    {
        try
    {
        var questionnaires = await _questionnaireRepository.GetActiveAsync();
            var dtos = _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
            return new JsonModel
            {
                Success = true,
                Data = dtos,
                Message = "Active questionnaires retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving active questionnaires: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByUserIdAsync(Guid userId)
    {
        try
    {
        var questionnaires = await _questionnaireRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
            return new JsonModel
            {
                Success = true,
                Data = dtos,
                Message = "User questionnaires retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving questionnaires by user: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> CreateAsync(CreateCategoryQuestionnaireTemplateDto createDto)
    {
        try
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId);
        if (category == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

        // Validate questions first (before checking for existing questionnaires)
        if (createDto.Questions != null && createDto.Questions.Any())
        {
            // Check for duplicate display orders
            var displayOrders = createDto.Questions.Select(q => q.DisplayOrder).ToList();
            var duplicateOrders = displayOrders.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateOrders.Any())
                    return new JsonModel
                    {
                        Success = false,
                        Message = $"Duplicate question order: {string.Join(", ", duplicateOrders)}",
                        StatusCode = HttpStatusCodes.BadRequest
                    };

            foreach (var questionDto in createDto.Questions)
            {
                // Validate question type exists
                var questionType = await _questionRepository.GetQuestionTypeByIdAsync(questionDto.QuestionTypeId);
                if (questionType == null)
                        return new JsonModel
                        {
                            Success = false,
                            Message = "Invalid question type",
                            StatusCode = HttpStatusCodes.BadRequest
                        };
            }
        }

        // Check if questionnaire already exists for this category
        var existingQuestionnaires = await _questionnaireRepository.GetByCategoryIdAsync(createDto.CategoryId);
        if (existingQuestionnaires.Any())
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template already exists for this category",
                    StatusCode = HttpStatusCodes.BadRequest
                };

        // Create questionnaire
        var questionnaire = _mapper.Map<CategoryQuestionnaireTemplate>(createDto);
        questionnaire.CreatedAt = DateTime.UtcNow;
        questionnaire.UpdatedAt = DateTime.UtcNow;
        
        // Set seeded admin user ID since authentication is disabled
        questionnaire.CreatedBy = AdminConstants.AdminUserId;
        
        questionnaire = await _questionnaireRepository.CreateAsync(questionnaire);

        // Create questions
        if (createDto.Questions != null && createDto.Questions.Any())
        {
            foreach (var questionDto in createDto.Questions)
            {
                var question = _mapper.Map<CategoryQuestion>(questionDto);
                question.QuestionnaireId = questionnaire.Id;
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
                    
                    // Create the question first
                    var createdQuestion = await _questionRepository.CreateAsync(question);
                    
                    // Create options if provided
                    if (questionDto.Options != null && questionDto.Options.Any())
                    {
                        foreach (var optionDto in questionDto.Options)
                        {
                            var option = new QuestionOption
                            {
                                Id = Guid.NewGuid(),
                                QuestionId = createdQuestion.Id,
                                OptionText = optionDto.OptionText,
                                OptionValue = optionDto.OptionValue,
                                DisplayOrder = optionDto.DisplayOrder,
                                IsCorrect = optionDto.IsCorrect,
                                IsActive = optionDto.IsActive,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            await _questionRepository.CreateOptionAsync(option);
                        }
                    }
            }
        }

        // Return the created questionnaire
        var createdQuestionnaire = await _questionnaireRepository.GetByIdAsync(questionnaire.Id);
        if (createdQuestionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Failed to retrieve created questionnaire",
                    StatusCode = HttpStatusCodes.InternalServerError
                };
                
            var dto = _mapper.Map<CategoryQuestionnaireTemplateDto>(createdQuestionnaire);
            return new JsonModel
            {
                Success = true,
                Data = dto,
                Message = "Questionnaire template created successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while creating the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> UpdateAsync(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto)
    {
        try
    {
        var existingQuestionnaire = await _questionnaireRepository.GetByIdAsync(id);
        if (existingQuestionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(updateDto.CategoryId);
        if (category == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

        // Update questionnaire properties
        _mapper.Map(updateDto, existingQuestionnaire);
        existingQuestionnaire.UpdatedAt = DateTime.UtcNow;
        existingQuestionnaire = await _questionnaireRepository.UpdateAsync(existingQuestionnaire);

        // Update questions
        if (updateDto.Questions != null && updateDto.Questions.Any())
        {
            // Delete existing questions
            var existingQuestions = await _questionRepository.GetByQuestionnaireIdAsync(id);
            foreach (var question in existingQuestions)
            {
                await _questionRepository.DeleteAsync(question.Id);
            }

            // Create new questions
            foreach (var questionDto in updateDto.Questions)
            {
                var question = _mapper.Map<CategoryQuestion>(questionDto);
                question.QuestionnaireId = id;
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
                    
                    // Create the question first
                    var createdQuestion = await _questionRepository.CreateAsync(question);
                    
                    // Create options if provided
                    if (questionDto.Options != null && questionDto.Options.Any())
                    {
                        foreach (var optionDto in questionDto.Options)
                        {
                            var option = new QuestionOption
                            {
                                Id = Guid.NewGuid(),
                                QuestionId = createdQuestion.Id,
                                OptionText = optionDto.OptionText,
                                OptionValue = optionDto.OptionValue,
                                DisplayOrder = optionDto.DisplayOrder,
                                IsCorrect = optionDto.IsCorrect,
                                IsActive = optionDto.IsActive,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            await _questionRepository.CreateOptionAsync(option);
                        }
                    }
            }
        }

        // Return the updated questionnaire
        var updatedQuestionnaire = await _questionnaireRepository.GetByIdAsync(id);
            var dto = _mapper.Map<CategoryQuestionnaireTemplateDto>(updatedQuestionnaire);
            return new JsonModel
            {
                Success = true,
                Data = dto,
                Message = "Questionnaire template updated successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while updating the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> DeleteAsync(Guid id)
    {
        try
    {
        // Check if questionnaire has responses
        var hasResponses = await _questionnaireRepository.HasResponsesAsync(id);
        if (hasResponses)
                return new JsonModel
                {
                    Success = false,
                    Message = "Cannot delete questionnaire template with existing responses",
                    StatusCode = HttpStatusCodes.BadRequest
                };

            var deleted = await _questionnaireRepository.DeleteAsync(id);
            if (!deleted)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            return new JsonModel
            {
                Success = true,
                Message = "Questionnaire template deleted successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while deleting the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> ExistsAsync(Guid id)
    {
        try
        {
            var exists = await _questionnaireRepository.ExistsAsync(id);
            return new JsonModel
            {
                Success = true,
                Data = exists,
                Message = "Existence check completed",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while checking existence: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetCountByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            var count = await _questionnaireRepository.GetCountByCategoryIdAsync(categoryId);
            return new JsonModel
            {
                Success = true,
                Data = count,
                Message = "Count retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while getting count: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> HasResponsesAsync(Guid questionnaireId)
    {
        try
        {
            var hasResponses = await _questionnaireRepository.HasResponsesAsync(questionnaireId);
            return new JsonModel
            {
                Success = true,
                Data = hasResponses,
                Message = "Response check completed",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while checking responses: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    // Question management methods
    public async Task<JsonModel> GetQuestionsByQuestionnaireIdAsync(Guid questionnaireId)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Get questions for this questionnaire
            var questions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);
            var questionDtos = questions.Select(q => new
            {
                id = q.Id,
                questionnaireId = q.QuestionnaireId,
                questionText = q.QuestionText,
                questionTypeId = q.QuestionTypeId,
                questionTypeName = q.QuestionType?.TypeName ?? "Unknown",
                isRequired = q.IsRequired,
                displayOrder = q.DisplayOrder,
                sectionName = q.SectionName,
                helpText = q.HelpText,
                placeholder = q.Placeholder,
                minLength = q.MinLength,
                maxLength = q.MaxLength,
                minValue = q.MinValue,
                maxValue = q.MaxValue,
                imageUrl = q.ImageUrl,
                imageAltText = q.ImageAltText,
                validationRules = q.ValidationRules,
                conditionalLogic = q.ConditionalLogic,
                settings = q.Settings,
                createdAt = q.CreatedAt,
                updatedAt = q.UpdatedAt,
                deletedAt = q.DeletedAt,
                options = q.Options == null ? new List<object>() : q.Options.Select(o => new
                {
                    id = o.Id,
                    questionId = o.QuestionId,
                    optionText = o.OptionText,
                    optionValue = o.OptionValue,
                    displayOrder = o.DisplayOrder,
                    isCorrect = o.IsCorrect,
                    isActive = o.IsActive,
                    createdAt = o.CreatedAt,
                    updatedAt = o.UpdatedAt
                }).Cast<object>().ToList()
            }).ToList();

            return new JsonModel
            {
                Success = true,
                Data = questionDtos,
                Message = "Questions retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving questions: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> AddQuestionAsync(Guid questionnaireId, CreateCategoryQuestionDto questionDto)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Auto-generate display order if not provided
            if (!questionDto.DisplayOrder.HasValue)
            {
                var maxOrder = await _questionRepository.GetMaxDisplayOrderAsync(questionnaireId);
                questionDto.DisplayOrder = maxOrder + 1;
            }
            else
            {
                // Check for duplicate display order if provided
                var existingQuestions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);
                if (existingQuestions.Any(q => q.DisplayOrder == questionDto.DisplayOrder))
                    return new JsonModel
                    {
                        Success = false,
                        Message = "Question with order " + questionDto.DisplayOrder + " already exists",
                        StatusCode = HttpStatusCodes.BadRequest
                    };
            }

            // Validate question type and options compatibility
            var questionType = await _questionRepository.GetQuestionTypeByIdAsync(questionDto.QuestionTypeId);
            if (questionType == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Invalid question type",
                    StatusCode = HttpStatusCodes.BadRequest
                };

            // Validate options for question types that require them
            if (questionType.HasOptions)
            {
                if (questionDto.Options == null || !questionDto.Options.Any())
                    return new JsonModel
                    {
                        Success = false,
                        Message = $"Question type '{questionType.DisplayName}' requires options",
                        StatusCode = HttpStatusCodes.BadRequest
                    };
            }

            // Create the question with options
            var question = _mapper.Map<CategoryQuestion>(questionDto);
            question.QuestionnaireId = questionnaireId;
            question.CreatedAt = DateTime.UtcNow;
            question.UpdatedAt = DateTime.UtcNow;

            var createdQuestion = await _questionRepository.CreateAsync(question);

            // Create options if provided (after question is created so we have the question ID)
            if (questionDto.Options != null && questionDto.Options.Any())
            {
                foreach (var optionDto in questionDto.Options)
                {
                    var option = new QuestionOption
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = createdQuestion.Id,
                        OptionText = optionDto.OptionText,
                        OptionValue = optionDto.OptionValue,
                        DisplayOrder = optionDto.DisplayOrder,
                        IsCorrect = optionDto.IsCorrect,
                        IsActive = optionDto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _questionRepository.CreateOptionAsync(option);
                }
            }

            return new JsonModel
            {
                Success = true,
                Data = createdQuestion,
                Message = "Question added successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while adding question: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> UpdateQuestionAsync(Guid questionnaireId, Guid questionId, UpdateCategoryQuestionDto questionDto)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
            if (existingQuestion == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Validate question type
            var questionType = await _questionRepository.GetQuestionTypeByIdAsync(questionDto.QuestionTypeId);
            if (questionType == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Invalid question type",
                    StatusCode = HttpStatusCodes.BadRequest
                };

            // Create a new question entity with updated properties to avoid concurrency issues
            var updatedQuestion = new CategoryQuestion
            {
                Id = existingQuestion.Id,
                QuestionnaireId = existingQuestion.QuestionnaireId,
                QuestionText = questionDto.QuestionText,
                QuestionTypeId = questionDto.QuestionTypeId,
                IsRequired = questionDto.IsRequired,
                DisplayOrder = questionDto.DisplayOrder ?? existingQuestion.DisplayOrder,
                SectionName = questionDto.SectionName,
                HelpText = questionDto.HelpText,
                Placeholder = questionDto.Placeholder,
                MinLength = questionDto.MinLength,
                MaxLength = questionDto.MaxLength,
                MinValue = questionDto.MinValue,
                MaxValue = questionDto.MaxValue,
                ImageUrl = questionDto.ImageUrl,
                ImageAltText = questionDto.ImageAltText,
                ValidationRules = questionDto.ValidationRules,
                ConditionalLogic = questionDto.ConditionalLogic,
                Settings = questionDto.Settings,
                CreatedAt = existingQuestion.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = existingQuestion.DeletedAt
            };

            var result = await _questionRepository.UpdateAsync(updatedQuestion);

            // Update options if provided
            if (questionDto.Options != null && questionDto.Options.Any())
            {
                // Clear any existing options first to prevent duplicates
                await _questionRepository.ClearQuestionOptionsAsync(questionId);
                
                // Create new options
                foreach (var optionDto in questionDto.Options)
                {
                    var option = new QuestionOption
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = questionId,
                        OptionText = optionDto.OptionText,
                        OptionValue = optionDto.OptionValue,
                        DisplayOrder = optionDto.DisplayOrder,
                        IsCorrect = optionDto.IsCorrect,
                        IsActive = optionDto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _questionRepository.CreateOptionAsync(option);
                }
            }

            return new JsonModel
            {
                Success = true,
                Data = result,
                Message = "Question updated successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while updating question: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> DeleteQuestionAsync(Guid questionnaireId, Guid questionId)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question has responses
            var hasResponses = await _questionRepository.HasResponsesAsync(questionId);
            if (hasResponses)
                return new JsonModel
                {
                    Success = false,
                    Message = "Cannot delete question with existing responses",
                    StatusCode = HttpStatusCodes.BadRequest
                };

            await _questionRepository.DeleteAsync(questionId);

            return new JsonModel
            {
                Success = true,
                Message = "Question deleted successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while deleting question: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetQuestionOptionsAsync(Guid questionnaireId, Guid questionId)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var options = await _questionRepository.GetOptionsByQuestionIdAsync(questionId);
            var optionDtos = _mapper.Map<IEnumerable<QuestionOptionDto>>(options);

            return new JsonModel
            {
                Success = true,
                Data = optionDtos,
                Message = "Question options retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving question options: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> AddQuestionOptionAsync(Guid questionnaireId, Guid questionId, CreateQuestionOptionDto optionDto)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var option = _mapper.Map<QuestionOption>(optionDto);
            option.QuestionId = questionId;
            option.CreatedAt = DateTime.UtcNow;
            option.UpdatedAt = DateTime.UtcNow;

            var createdOption = await _questionRepository.CreateOptionAsync(option);

            return new JsonModel
            {
                Success = true,
                Data = createdOption,
                Message = "Question option added successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while adding question option: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> UpdateQuestionOptionAsync(Guid questionnaireId, Guid questionId, Guid optionId, UpdateQuestionOptionDto optionDto)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if option exists
            var option = await _questionRepository.GetOptionByIdAsync(optionId);
            if (option == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question option not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            _mapper.Map(optionDto, option);
            option.UpdatedAt = DateTime.UtcNow;
            var updatedOption = await _questionRepository.UpdateOptionAsync(option);

            return new JsonModel
            {
                Success = true,
                Data = updatedOption,
                Message = "Question option updated successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while updating question option: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> DeleteQuestionOptionAsync(Guid questionnaireId, Guid questionId, Guid optionId)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // Check if option exists
            var option = await _questionRepository.GetOptionByIdAsync(optionId);
            if (option == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question option not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            await _questionRepository.DeleteOptionAsync(optionId);

            return new JsonModel
            {
                Success = true,
                Message = "Question option deleted successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while deleting question option: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> UpdateQuestionOrderAsync(Guid questionnaireId, List<QuestionOrderUpdateDto> orderUpdates)
    {
        try
        {
            // Check if questionnaire exists
            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            if (orderUpdates == null || !orderUpdates.Any())
                return new JsonModel
                {
                    Success = false,
                    Message = "Order updates are required",
                    StatusCode = HttpStatusCodes.BadRequest
                };

            // Validate that all questions exist and belong to this questionnaire
            foreach (var orderUpdate in orderUpdates)
            {
                var question = await _questionRepository.GetByIdAsync(orderUpdate.Id);
                if (question == null || question.QuestionnaireId != questionnaireId)
                    return new JsonModel
                    {
                        Success = false,
                        Message = $"Question with ID {orderUpdate.Id} not found",
                        StatusCode = HttpStatusCodes.NotFound
                    };
            }

            // Update question orders
            foreach (var orderUpdate in orderUpdates)
            {
                var question = await _questionRepository.GetByIdAsync(orderUpdate.Id);
                question.DisplayOrder = orderUpdate.DisplayOrder;
                question.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(question);
            }

            return new JsonModel
            {
                Success = true,
                Message = "Question order updated successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"An error occurred while updating question order: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }
} 