using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Helpers;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using System.Linq;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IUserQuestionResponseService _responseService;

    public ResponsesController(IUserQuestionResponseService responseService)
    {
        _responseService = responseService;
    }

    [HttpGet]
    public async Task<ActionResult<JsonModel>> GetUserResponses()
    {
        return await _responseService.GetUserResponsesAsync(TokenHelper.GetToken(HttpContext));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetResponseById(Guid id)
    {
        var result = await _responseService.GetResponseByIdAsync(id, TokenHelper.GetToken(HttpContext));
        
        if (!result.Success && result.StatusCode == 404)
            return NotFound(result);
        
        return Ok(result);
    }

    [HttpGet("questionnaire/{questionnaireId:guid}")]
    public async Task<ActionResult<JsonModel>> GetResponsesByQuestionnaire(Guid questionnaireId)
    {
        return await _responseService.GetResponsesByQuestionnaireAsync(questionnaireId, TokenHelper.GetToken(HttpContext));
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> SubmitResponse(SubmitResponseDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(JsonModel.ErrorResult($"Validation failed: {string.Join(", ", errors)}", HttpStatusCodes.BadRequest));
        }
        
        return await _responseService.SubmitResponseAsync(dto, TokenHelper.GetToken(HttpContext));
    }

    [HttpPost("validate")]
    public async Task<ActionResult<JsonModel>> ValidateResponses(SubmitResponseDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(JsonModel.ErrorResult($"Validation failed: {string.Join(", ", errors)}", HttpStatusCodes.BadRequest));
        }
        
        return await _responseService.ValidateResponsesAsync(dto, TokenHelper.GetToken(HttpContext));
    }
} 