using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using System.Linq;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var result = await _responseService.GetUserResponsesAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<JsonModel>> GetAllResponses()
    {
        var result = await _responseService.GetAllResponsesAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetResponseById(Guid id)
    {
        var result = await _responseService.GetResponseByIdAsync(id, new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("questionnaire/{questionnaireId:guid}")]
    public async Task<ActionResult<JsonModel>> GetResponsesByQuestionnaire(Guid questionnaireId)
    {
        var result = await _responseService.GetResponsesByQuestionnaireAsync(questionnaireId, new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> SubmitResponse(SubmitResponseDto dto)
    {
        var result = await _responseService.SubmitResponseAsync(dto, new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default" // This will allow access to any questionnaire
        });
        return Ok(result);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<JsonModel>> ValidateResponses(SubmitResponseDto dto)
    {
        var result = await _responseService.ValidateResponsesAsync(dto, new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("export/{questionnaireId:guid}")]
    public async Task<ActionResult<JsonModel>> ExportResponses(Guid questionnaireId)
    {
        var result = await _responseService.ExportResponsesAsync(questionnaireId, new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    // Analytics Endpoints
    [HttpGet("analytics/summary")]
    public async Task<ActionResult<JsonModel>> GetAnalyticsSummary()
    {
        var result = await _responseService.GetAnalyticsSummaryAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("analytics/trends")]
    public async Task<ActionResult<JsonModel>> GetAnalyticsTrends()
    {
        var result = await _responseService.GetAnalyticsTrendsAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("analytics/categories")]
    public async Task<ActionResult<JsonModel>> GetCategoryAnalytics()
    {
        var result = await _responseService.GetCategoryAnalyticsAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }

    [HttpGet("analytics/questionnaires")]
    public async Task<ActionResult<JsonModel>> GetQuestionnaireAnalytics()
    {
        var result = await _responseService.GetQuestionnaireAnalyticsAsync(new TokenModel { 
            UserId = AdminConstants.AdminUserId,
            Role = AdminConstants.AdminRole,
            Category = "Default"
        });
        return Ok(result);
    }
} 