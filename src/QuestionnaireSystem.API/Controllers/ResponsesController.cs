using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.API.Services;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IPatientResponseService _responseService;

    public ResponsesController(IPatientResponseService responseService)
    {
        _responseService = responseService;
    }

    [HttpGet("assignment/{assignmentId:guid}")]
    public async Task<ActionResult<ResponseSummaryDto>> GetByAssignmentId(Guid assignmentId)
    {
        var response = await _responseService.GetByAssignmentIdAsync(assignmentId);
        if (response == null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<IEnumerable<ResponseSummaryDto>>> GetByPatientId(Guid patientId)
    {
        var responses = await _responseService.GetByPatientIdAsync(patientId);
        return Ok(responses);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseSummaryDto>> SaveResponses(SaveResponsesDto dto)
    {
        var response = await _responseService.SaveResponsesAsync(dto);
        return CreatedAtAction(nameof(GetByAssignmentId), new { assignmentId = response.AssignmentId }, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _responseService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
} 