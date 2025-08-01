using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Helpers;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        return await _questionTypeService.GetAllAsync(TokenHelper.GetToken(HttpContext));
    }

    [HttpGet("active")]
    public async Task<ActionResult<JsonModel>> GetActive()
    {
        return await _questionTypeService.GetActiveAsync(TokenHelper.GetToken(HttpContext));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        return await _questionTypeService.GetByIdAsync(id, TokenHelper.GetToken(HttpContext));
    }
} 