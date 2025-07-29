using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Services;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionnairesController : ControllerBase
{
    private readonly IQuestionnaireService _questionnaireService;

    public QuestionnairesController(IQuestionnaireService questionnaireService)
    {
        _questionnaireService = questionnaireService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionnaireDetailDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var questionnaires = await _questionnaireService.GetAllAsync(includeInactive);
        return Ok(questionnaires);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<ActionResult<IEnumerable<QuestionnaireDetailDto>>> GetByCategory(Guid categoryId)
    {
        var questionnaires = await _questionnaireService.GetByCategoryAsync(categoryId);
        return Ok(questionnaires);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuestionnaireDetailDto>> GetById(Guid id)
    {
        var questionnaire = await _questionnaireService.GetByIdAsync(id);
        if (questionnaire == null)
            return NotFound();

        return Ok(questionnaire);
    }

    [HttpGet("{id:guid}/response")]
    [AllowAnonymous]
    public async Task<ActionResult<QuestionnaireResponseDto>> GetForResponse(Guid id)
    {
        var questionnaire = await _questionnaireService.GetForResponseAsync(id);
        if (questionnaire == null)
            return NotFound();

        return Ok(questionnaire);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<QuestionnaireDetailDto>> Create(CreateQuestionnaireDto dto)
    {
        // TODO: Get actual user ID from JWT token
        var createdBy = Guid.NewGuid(); // Placeholder

        var questionnaire = await _questionnaireService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = questionnaire.Id }, questionnaire);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<QuestionnaireDetailDto>> Update(Guid id, UpdateQuestionnaireDto dto)
    {
        var questionnaire = await _questionnaireService.UpdateAsync(id, dto);
        return Ok(questionnaire);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _questionnaireService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
} 