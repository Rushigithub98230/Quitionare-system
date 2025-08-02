using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.API.Helpers;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        return await _questionnaireService.GetAllAsync();
    }

    [HttpGet("active")]
    public async Task<ActionResult<JsonModel>> GetActive()
    {
        return await _questionnaireService.GetActiveAsync();
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<JsonModel>> GetByCategoryId(Guid categoryId)
    {
        return await _questionnaireService.GetByCategoryIdAsync(categoryId);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<JsonModel>> GetByUserId(Guid userId)
    {
        return await _questionnaireService.GetByUserIdAsync(userId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdAsync(id);
            if (questionnaire == null)
                return NotFound(new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                });

            return Ok(new JsonModel
            {
                Success = true,
                Data = questionnaire,
                Message = "Questionnaire template retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            });
        }
    }

    [HttpGet("{id}/with-category")]
    public async Task<ActionResult<JsonModel>> GetByIdWithCategory(Guid id)
    {
        try
        {
            var questionnaire = await _questionnaireService.GetByIdWithCategoryAsync(id);
            if (questionnaire == null)
                return NotFound(new JsonModel
                {
                    Success = false,
                    Message = "Questionnaire template not found",
                    StatusCode = HttpStatusCodes.NotFound
                });

            return Ok(new JsonModel
            {
                Success = true,
                Data = questionnaire,
                Message = "Questionnaire with category retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new JsonModel
            {
                Success = false,
                Message = $"An error occurred while retrieving the questionnaire: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            });
        }
    }

    [HttpGet("{id}/with-responses")]
    public async Task<ActionResult<JsonModel>> GetByIdWithResponses(Guid id)
    {
        return await _questionnaireService.GetByIdWithResponsesAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> Create(CreateCategoryQuestionnaireTemplateDto createDto)
    {
        return await _questionnaireService.CreateAsync(createDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JsonModel>> Update(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto)
    {
        return await _questionnaireService.UpdateAsync(id, updateDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<JsonModel>> Delete(Guid id)
    {
        return await _questionnaireService.DeleteAsync(id);
    }

    [HttpGet("category/{categoryId}/count")]
    public async Task<ActionResult<JsonModel>> GetCountByCategoryId(Guid categoryId)
    {
        return await _questionnaireService.GetCountByCategoryIdAsync(categoryId);
    }

    [HttpGet("{id}/response")]
    public async Task<ActionResult<JsonModel>> GetForResponse(Guid id)
    {
        return await _questionnaireService.GetByIdAsync(id);
    }

    [HttpGet("{questionnaireId}/questions")]
    public async Task<ActionResult<JsonModel>> GetQuestionsByQuestionnaireId(Guid questionnaireId)
    {
        return await _questionnaireService.GetQuestionsByQuestionnaireIdAsync(questionnaireId);
    }

    // Question management endpoints
    [HttpPost("{questionnaireId}/questions")]
    public async Task<ActionResult<JsonModel>> AddQuestion(Guid questionnaireId, CreateCategoryQuestionDto questionDto)
    {
        return await _questionnaireService.AddQuestionAsync(questionnaireId, questionDto);
    }

    [HttpPut("{questionnaireId}/questions/{questionId}")]
    public async Task<ActionResult<JsonModel>> UpdateQuestion(Guid questionnaireId, Guid questionId, UpdateCategoryQuestionDto questionDto)
    {
        return await _questionnaireService.UpdateQuestionAsync(questionnaireId, questionId, questionDto);
    }

    [HttpDelete("{questionnaireId}/questions/{questionId}")]
    public async Task<ActionResult<JsonModel>> DeleteQuestion(Guid questionnaireId, Guid questionId)
    {
        return await _questionnaireService.DeleteQuestionAsync(questionnaireId, questionId);
    }

    // Question Options endpoints
    [HttpGet("{questionnaireId}/questions/{questionId}/options")]
    public async Task<ActionResult<JsonModel>> GetQuestionOptions(Guid questionnaireId, Guid questionId)
    {
        return await _questionnaireService.GetQuestionOptionsAsync(questionnaireId, questionId);
    }

    [HttpPost("{questionnaireId}/questions/{questionId}/options")]
    public async Task<ActionResult<JsonModel>> AddQuestionOption(Guid questionnaireId, Guid questionId, CreateQuestionOptionDto optionDto)
    {
        return await _questionnaireService.AddQuestionOptionAsync(questionnaireId, questionId, optionDto);
    }

    [HttpPut("{questionnaireId}/questions/{questionId}/options/{optionId}")]
    public async Task<ActionResult<JsonModel>> UpdateQuestionOption(Guid questionnaireId, Guid questionId, Guid optionId, UpdateQuestionOptionDto optionDto)
    {
        return await _questionnaireService.UpdateQuestionOptionAsync(questionnaireId, questionId, optionId, optionDto);
    }

    [HttpDelete("{questionnaireId}/questions/{questionId}/options/{optionId}")]
    public async Task<ActionResult<JsonModel>> DeleteQuestionOption(Guid questionnaireId, Guid questionId, Guid optionId)
    {
        return await _questionnaireService.DeleteQuestionOptionAsync(questionnaireId, questionId, optionId);
    }

    // Question order management endpoint
    [HttpPut("{questionnaireId}/questions/order")]
    public async Task<ActionResult<JsonModel>> UpdateQuestionOrder(Guid questionnaireId, List<QuestionOrderUpdateDto> orderUpdates)
    {
        return await _questionnaireService.UpdateQuestionOrderAsync(questionnaireId, orderUpdates);
    }
} 