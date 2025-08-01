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
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<JsonModel>> GetAll()
    {
        return await _categoryService.GetAllAsync(TokenHelper.GetToken(HttpContext));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        return await _categoryService.GetByIdAsync(id, TokenHelper.GetToken(HttpContext));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<JsonModel>> Create(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(JsonModel.ErrorResult($"Validation failed: {string.Join(", ", errors)}", HttpStatusCodes.BadRequest));
        }
        
        return await _categoryService.CreateAsync(dto, TokenHelper.GetToken(HttpContext));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<JsonModel>> Update(Guid id, UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(JsonModel.ErrorResult($"Validation failed: {string.Join(", ", errors)}", HttpStatusCodes.BadRequest));
        }
        
        return await _categoryService.UpdateAsync(id, dto, TokenHelper.GetToken(HttpContext));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<JsonModel>> Delete(Guid id)
    {
        return await _categoryService.DeleteAsync(id, TokenHelper.GetToken(HttpContext));
    }
} 