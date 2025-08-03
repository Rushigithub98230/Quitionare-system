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
        // TODO: Re-enable authentication for production
        return await _categoryService.GetAllAsync(null);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<JsonModel>> GetDeleted()
    {
        // TODO: Re-enable authentication for production
        return await _categoryService.GetDeletedAsync(null);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JsonModel>> GetById(Guid id)
    {
        // TODO: Re-enable authentication for production
        return await _categoryService.GetByIdAsync(id, null);
    }

    [HttpGet("check-name/{name}")]
    public async Task<ActionResult<JsonModel>> CheckNameExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new JsonModel
            {
                Success = false,
                Message = "Category name is required",
                StatusCode = HttpStatusCodes.BadRequest
            });
        }

        // TODO: Re-enable authentication for production
        return await _categoryService.CheckNameExistsAsync(name, null);
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> Create(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new JsonModel
            {
                Success = false,
                Message = $"Validation failed: {string.Join(", ", errors)}",
                StatusCode = HttpStatusCodes.BadRequest
            });
        }
        
        // TODO: Re-enable authentication for production
        return await _categoryService.CreateAsync(dto, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JsonModel>> Update(Guid id, UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new JsonModel
            {
                Success = false,
                Message = $"Validation failed: {string.Join(", ", errors)}",
                StatusCode = HttpStatusCodes.BadRequest
            });
        }
        
        // TODO: Re-enable authentication for production
        return await _categoryService.UpdateAsync(id, dto, null);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<JsonModel>> Delete(Guid id)
    {
        // TODO: Re-enable authentication for production
        return await _categoryService.DeleteAsync(id, null);
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<JsonModel>> Restore(Guid id)
    {
        // TODO: Re-enable authentication for production
        return await _categoryService.RestoreAsync(id, null);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult<JsonModel>> Deactivate(Guid id)
    {
        return await _categoryService.DeactivateAsync(id, null);
    }

    [HttpPost("{id}/reactivate")]
    public async Task<ActionResult<JsonModel>> Reactivate(Guid id)
    {
        return await _categoryService.ReactivateAsync(id, null);
    }

    [HttpGet("deactivated")]
    public async Task<ActionResult<JsonModel>> GetDeactivated()
    {
        return await _categoryService.GetDeactivatedAsync(null);
    }

    // Category order management endpoint
    [HttpPut("order")]
    public async Task<ActionResult<JsonModel>> UpdateOrder(List<CategoryOrderUpdateDto> orderUpdates)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Check if user is admin
            // var tokenModel = TokenHelper.GetToken(HttpContext);
            // if (tokenModel.Role != "Admin")
            //     return StatusCode(403, JsonModel.ErrorResult("Access denied. Admin role required.", HttpStatusCodes.Forbidden));

            // Validate order updates
            if (orderUpdates == null || !orderUpdates.Any())
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = "Order updates are required",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            // Check for duplicate display orders
            var displayOrders = orderUpdates.Select(u => u.DisplayOrder).ToList();
            var duplicateOrders = displayOrders.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateOrders.Any())
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = $"Duplicate category order: {string.Join(", ", duplicateOrders)}",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            // Update each category's display order
            var updatedCategories = new List<Category>();
            foreach (var orderUpdate in orderUpdates)
            {
                var category = await _categoryService.GetByIdAsync(orderUpdate.Id, null);
                if (category == null || !category.Success)
                    return NotFound(new JsonModel
                    {
                        Success = false,
                        Message = $"Category with ID {orderUpdate.Id} not found",
                        StatusCode = HttpStatusCodes.NotFound
                    });

                var categoryData = category.Data as CategoryDto;
                if (categoryData == null)
                    return NotFound(new JsonModel
                    {
                        Success = false,
                        Message = $"Category with ID {orderUpdate.Id} not found",
                        StatusCode = HttpStatusCodes.NotFound
                    });

                // Update the category's display order
                var updateDto = new UpdateCategoryDto
                {
                    Name = categoryData.Name,
                    Description = categoryData.Description,
                    Icon = categoryData.Icon,
                    Color = categoryData.Color,
                    DisplayOrder = orderUpdate.DisplayOrder,
                    Features = categoryData.Features,
                    ConsultationDescription = categoryData.ConsultationDescription,
                    BasePrice = categoryData.BasePrice,
                    RequiresQuestionnaireAssessment = categoryData.RequiresQuestionnaireAssessment,
                    AllowsMedicationDelivery = categoryData.AllowsMedicationDelivery,
                    AllowsFollowUpMessaging = categoryData.AllowsFollowUpMessaging,
                    OneTimeConsultationDurationMinutes = categoryData.OneTimeConsultationDurationMinutes,
                    IsMostPopular = categoryData.IsMostPopular,
                    IsTrending = categoryData.IsTrending,
                    IsActive = categoryData.IsActive
                };

                var updateResult = await _categoryService.UpdateAsync(orderUpdate.Id, updateDto, null);
                if (!updateResult.Success)
                    return StatusCode(500, new JsonModel
                    {
                        Success = false,
                        Message = $"Failed to update category {orderUpdate.Id}: {updateResult.Message}",
                        StatusCode = HttpStatusCodes.InternalServerError
                    });

                var updatedCategory = updateResult.Data as CategoryDto;
                if (updatedCategory != null)
                {
                    // Convert DTO back to model for response
                    updatedCategories.Add(new Category
                    {
                        Id = updatedCategory.Id,
                        Name = updatedCategory.Name,
                        Description = updatedCategory.Description,
                        Icon = updatedCategory.Icon,
                        Color = updatedCategory.Color,
                        DisplayOrder = updatedCategory.DisplayOrder,
                        Features = updatedCategory.Features,
                        ConsultationDescription = updatedCategory.ConsultationDescription,
                        BasePrice = updatedCategory.BasePrice,
                        RequiresQuestionnaireAssessment = updatedCategory.RequiresQuestionnaireAssessment,
                        AllowsMedicationDelivery = updatedCategory.AllowsMedicationDelivery,
                        AllowsFollowUpMessaging = updatedCategory.AllowsFollowUpMessaging,
                        OneTimeConsultationDurationMinutes = updatedCategory.OneTimeConsultationDurationMinutes,
                        IsMostPopular = updatedCategory.IsMostPopular,
                        IsTrending = updatedCategory.IsTrending,
                        IsActive = updatedCategory.IsActive,
                        CreatedAt = updatedCategory.CreatedAt,
                        UpdatedAt = updatedCategory.UpdatedAt
                    });
                }
            }

            // Return the updated categories
            var categoryDtos = updatedCategories.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                description = c.Description,
                icon = c.Icon,
                color = c.Color,
                displayOrder = c.DisplayOrder,
                features = c.Features,
                consultationDescription = c.ConsultationDescription,
                basePrice = c.BasePrice,
                requiresQuestionnaireAssessment = c.RequiresQuestionnaireAssessment,
                allowsMedicationDelivery = c.AllowsMedicationDelivery,
                allowsFollowUpMessaging = c.AllowsFollowUpMessaging,
                oneTimeConsultationDurationMinutes = c.OneTimeConsultationDurationMinutes,
                isMostPopular = c.IsMostPopular,
                isTrending = c.IsTrending,
                isActive = c.IsActive,
                createdAt = c.CreatedAt,
                updatedAt = c.UpdatedAt,
                deletedAt = c.DeletedAt
            }).ToList();

            return Ok(new JsonModel
            {
                Success = true,
                Data = categoryDtos,
                Message = "Category order updated successfully",
                StatusCode = HttpStatusCodes.OK
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new JsonModel
            {
                Success = false,
                Message = $"An error occurred while updating category order: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            });
        }
    }
} 