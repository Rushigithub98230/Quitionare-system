using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.API.Helpers;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryQuestionnaireTemplatesController : ControllerBase
{
    private readonly ICategoryQuestionnaireTemplateService _questionnaireService;
    private readonly ICategoryQuestionRepository _questionRepository;

    public CategoryQuestionnaireTemplatesController(
        ICategoryQuestionnaireTemplateService questionnaireService,
        ICategoryQuestionRepository questionRepository)
    {
        _questionnaireService = questionnaireService;
        _questionRepository = questionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<JsonModel>> GetAll()
    {
        try
        {
            var questionnaires = await _questionnaireService.GetAllAsync();
            return Ok(JsonModel.SuccessResult(questionnaires, "Questionnaire templates retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving questionnaires: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<JsonModel>> GetActive()
    {
        try
        {
            var questionnaires = await _questionnaireService.GetActiveAsync();
            return Ok(JsonModel.SuccessResult(questionnaires, "Active questionnaires retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving active questionnaires: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<JsonModel>> GetByCategoryId(Guid categoryId)
    {
        try
        {
            var questionnaires = await _questionnaireService.GetByCategoryIdAsync(categoryId);
            if (!questionnaires.Any())
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found for this category"));
            
            return Ok(JsonModel.SuccessResult(questionnaires, "Questionnaire template retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving questionnaires by category: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<JsonModel>> GetByUserId(Guid userId)
    {
        try
        {
            var questionnaires = await _questionnaireService.GetByUserIdAsync(userId);
            return Ok(JsonModel.SuccessResult(questionnaires, "User questionnaires retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving questionnaires by user: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdAsync(id);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            return Ok(JsonModel.SuccessResult(questionnaire, "Questionnaire template retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("{id}/with-category")]
    public async Task<ActionResult<JsonModel>> GetByIdWithCategory(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdWithCategoryAsync(id);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            return Ok(JsonModel.SuccessResult(questionnaire, "Questionnaire with category retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("{id}/with-responses")]
    public async Task<ActionResult<JsonModel>> GetByIdWithResponses(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdWithResponsesAsync(id);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            return Ok(JsonModel.SuccessResult(questionnaire, "Questionnaire with responses retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> Create(CreateCategoryQuestionnaireTemplateDto createDto)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            var questionnaire = await _questionnaireService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = questionnaire.Id }, 
                JsonModel.SuccessResult(questionnaire, "Questionnaire template created successfully"));
        }
        catch (ArgumentException ex) when (ex.Message == "Category not found")
        {
            return NotFound(JsonModel.NotFoundResult("Category not found"));
        }
        catch (ArgumentException ex) when (ex.Message.StartsWith("Questionnaire template already exists"))
        {
            return BadRequest(JsonModel.ValidationErrorResult(ex.Message));
        }
        catch (ArgumentException ex) when (ex.Message.StartsWith("Invalid question type"))
        {
            return BadRequest(JsonModel.ValidationErrorResult(ex.Message));
        }
        catch (ArgumentException ex) when (ex.Message.StartsWith("Duplicate question order"))
        {
            return BadRequest(JsonModel.ValidationErrorResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(JsonModel.ValidationErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while creating the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JsonModel>> Update(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            var questionnaire = await _questionnaireService.UpdateAsync(id, updateDto);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            return Ok(JsonModel.SuccessResult(questionnaire, "Questionnaire template updated successfully"));
        }
        catch (ArgumentException ex) when (ex.Message == "Category not found")
        {
            return NotFound(JsonModel.NotFoundResult("Category not found"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(JsonModel.ValidationErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while updating the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<JsonModel>> Delete(Guid id)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            // Check if questionnaire has responses
            var hasResponses = await _questionnaireService.HasResponsesAsync(id);
            if (hasResponses)
                return BadRequest(JsonModel.ValidationErrorResult("Cannot delete questionnaire template with existing responses"));

            var deleted = await _questionnaireService.DeleteAsync(id);
            if (!deleted)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            return Ok(JsonModel.SuccessResult(null, "Questionnaire template deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while deleting the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("category/{categoryId}/count")]
    public async Task<ActionResult<JsonModel>> GetCountByCategoryId(Guid categoryId)
    {
        try
        {
            var count = await _questionnaireService.GetCountByCategoryIdAsync(categoryId);
            return Ok(JsonModel.SuccessResult(count, "Questionnaire count retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving questionnaire count: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpGet("{id}/response")]
    public async Task<ActionResult<JsonModel>> GetForResponse(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdAsync(id);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found or inactive"));

            if (!questionnaire.IsActive)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found or inactive"));

            return Ok(JsonModel.SuccessResult(questionnaire, "Questionnaire template retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while retrieving the questionnaire: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    // Question management endpoints
    [HttpPost("{questionnaireId}/questions")]
    public async Task<ActionResult<JsonModel>> AddQuestion(Guid questionnaireId, CreateCategoryQuestionDto questionDto)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            // Check if questionnaire exists
            var questionnaire = await _questionnaireService.GetByIdAsync(questionnaireId);
            if (questionnaire == null)
                return NotFound(JsonModel.NotFoundResult("Questionnaire template not found"));

            // Check for duplicate display order
            var existingQuestions = await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId);
            if (existingQuestions.Any(q => q.DisplayOrder == questionDto.DisplayOrder))
                return BadRequest(JsonModel.ValidationErrorResult("Question with order " + questionDto.DisplayOrder + " already exists"));

            // Create the question
            questionDto.QuestionnaireId = questionnaireId;
            var question = new CategoryQuestion
            {
                Id = Guid.NewGuid(),
                QuestionnaireId = questionnaireId,
                QuestionText = questionDto.QuestionText,
                QuestionTypeId = questionDto.QuestionTypeId,
                IsRequired = questionDto.IsRequired,
                DisplayOrder = questionDto.DisplayOrder,
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _questionRepository.CreateAsync(question);

            return CreatedAtAction(nameof(GetById), new { id = questionnaireId }, 
                JsonModel.SuccessResult(question, "Question added successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while adding the question: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpPut("{questionnaireId}/questions/{questionId}")]
    public async Task<ActionResult<JsonModel>> UpdateQuestion(Guid questionnaireId, Guid questionId, UpdateCategoryQuestionDto questionDto)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            // Check if question exists
            var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
            if (existingQuestion == null || existingQuestion.QuestionnaireId != questionnaireId)
                return NotFound(JsonModel.NotFoundResult("Question not found"));

            // Update the question
            existingQuestion.QuestionText = questionDto.QuestionText;
            existingQuestion.QuestionTypeId = questionDto.QuestionTypeId;
            existingQuestion.IsRequired = questionDto.IsRequired;
            existingQuestion.DisplayOrder = questionDto.DisplayOrder;
            existingQuestion.SectionName = questionDto.SectionName;
            existingQuestion.HelpText = questionDto.HelpText;
            existingQuestion.Placeholder = questionDto.Placeholder;
            existingQuestion.MinLength = questionDto.MinLength;
            existingQuestion.MaxLength = questionDto.MaxLength;
            existingQuestion.MinValue = questionDto.MinValue;
            existingQuestion.MaxValue = questionDto.MaxValue;
            existingQuestion.ImageUrl = questionDto.ImageUrl;
            existingQuestion.ImageAltText = questionDto.ImageAltText;
            existingQuestion.ValidationRules = questionDto.ValidationRules;
            existingQuestion.ConditionalLogic = questionDto.ConditionalLogic;
            existingQuestion.Settings = questionDto.Settings;
            existingQuestion.UpdatedAt = DateTime.UtcNow;

            await _questionRepository.UpdateAsync(existingQuestion);

            return Ok(JsonModel.SuccessResult(existingQuestion, "Question updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while updating the question: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }

    [HttpDelete("{questionnaireId}/questions/{questionId}")]
    public async Task<ActionResult<JsonModel>> DeleteQuestion(Guid questionnaireId, Guid questionId)
    {
        try
        {
            // Check if user is admin
            var tokenModel = TokenHelper.GetToken(HttpContext);
            if (tokenModel.Role != "Admin")
                return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            // Check if question exists
            var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
            if (existingQuestion == null || existingQuestion.QuestionnaireId != questionnaireId)
                return NotFound(JsonModel.NotFoundResult("Question not found"));

            // Check if question has responses (if so, return error)
            var hasResponses = await _questionRepository.HasResponsesAsync(questionId);
            if (hasResponses)
                return BadRequest(JsonModel.ValidationErrorResult("Cannot delete question with existing responses"));

            await _questionRepository.DeleteAsync(questionId);

            return Ok(JsonModel.SuccessResult(null, "Question deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, JsonModel.ErrorResult($"An error occurred while deleting the question: {ex.Message}", HttpStatusCodes.InternalServerError));
        }
    }
} 