using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Helpers;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionTypesController : ControllerBase
{
    private readonly IQuestionTypeService _questionTypeService;

    public QuestionTypesController(IQuestionTypeService questionTypeService)
    {
        _questionTypeService = questionTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<JsonModel>> GetAll()
    {
        // TODO: Re-enable authentication for production
        return await _questionTypeService.GetAllAsync(null);
    }

    [HttpGet("active")]
    public async Task<ActionResult<JsonModel>> GetActive()
    {
        // TODO: Re-enable authentication for production
        return await _questionTypeService.GetActiveAsync(null);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        // TODO: Re-enable authentication for production
        return await _questionTypeService.GetByIdAsync(id, null);
    }
} 