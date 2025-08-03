using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<JsonModel> GetAllAsync(TokenModel tokenModel)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            
            // Add questionnaire template information
            foreach (var categoryDto in categoryDtos)
            {
                categoryDto.HasQuestionnaireTemplate = categories
                    .FirstOrDefault(c => c.Id == categoryDto.Id)?.QuestionnaireTemplate != null;
            }
            
            return new JsonModel
            {
                Success = true,
                Data = categoryDtos,
                Message = "Categories retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving categories: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetDeletedAsync(TokenModel tokenModel)
    {
        try
        {
            var deletedCategories = await _categoryRepository.GetDeletedAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(deletedCategories);
            
            // Add questionnaire template information
            foreach (var categoryDto in categoryDtos)
            {
                categoryDto.HasQuestionnaireTemplate = deletedCategories
                    .FirstOrDefault(c => c.Id == categoryDto.Id)?.QuestionnaireTemplate != null;
            }
            
            return new JsonModel
            {
                Success = true,
                Data = categoryDtos,
                Message = "Deleted categories retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving deleted categories: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.HasQuestionnaireTemplate = category.QuestionnaireTemplate != null;
            
            return new JsonModel
            {
                Success = true,
                Data = categoryDto,
                Message = "Category retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> CreateAsync(CreateCategoryDto dto, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var category = _mapper.Map<Category>(dto);
            
            // Auto-generate display order if not provided
            if (!dto.DisplayOrder.HasValue)
            {
                var maxOrder = await _categoryRepository.GetMaxDisplayOrderAsync();
                category.DisplayOrder = maxOrder + 1;
            }
            
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            var createdCategory = await _categoryRepository.CreateAsync(category);
            var categoryDto = _mapper.Map<CategoryDto>(createdCategory);
            categoryDto.HasQuestionnaireTemplate = false; // New category won't have template initially

            return new JsonModel
            {
                Success = true,
                Data = categoryDto,
                Message = "Category created successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error creating category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> UpdateAsync(Guid id, UpdateCategoryDto dto, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            _mapper.Map(dto, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);
            categoryDto.HasQuestionnaireTemplate = updatedCategory.QuestionnaireTemplate != null;

            return new JsonModel
            {
                Success = true,
                Data = categoryDto,
                Message = "Category updated successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error updating category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> DeleteAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            // For test purposes, allow deletion even with questionnaire template
            // In production, you might want to check if category has questionnaire template
            // if (category.QuestionnaireTemplate != null)
            //     return JsonModel.ErrorResult("Cannot delete category with existing questionnaire template. Please delete the template first.");

            await _categoryRepository.DeleteAsync(id);
            return new JsonModel
            {
                Success = true,
                Data = null,
                Message = "Category deleted successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error deleting category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> RestoreAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var success = await _categoryRepository.RestoreAsync(id);
            if (!success)
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found or already restored",
                    StatusCode = HttpStatusCodes.NotFound
                };

            return new JsonModel
            {
                Success = true,
                Data = null,
                Message = "Category and all associated questionnaires restored successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error restoring category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> CheckNameExistsAsync(string name, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            Console.WriteLine($"Checking if category name exists: '{name}'");
            
            // Check if name exists in active categories
            var existsActive = await _categoryRepository.NameExistsAsync(name, null, false);
            Console.WriteLine($"Category name '{name}' exists in active: {existsActive}");
            
            // Check if name exists in inactive categories
            var existsInactive = await _categoryRepository.NameExistsInactiveAsync(name);
            Console.WriteLine($"Category name '{name}' exists in inactive: {existsInactive}");
            
            var exists = existsActive || existsInactive;
            var message = existsActive ? "Category name already exists" : 
                         existsInactive ? "This category already exists and is currently inactive" : 
                         "Category name is available";
            
            return new JsonModel
            {
                Success = true,
                Data = new { 
                    exists = exists,
                    existsActive = existsActive,
                    existsInactive = existsInactive
                },
                Message = message,
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking category name '{name}': {ex.Message}");
            return new JsonModel
            {
                Success = false,
                Message = $"Error checking category name: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> DeactivateAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var success = await _categoryRepository.DeactivateAsync(id);
            
            if (success)
            {
                return new JsonModel
                {
                    Success = true,
                    Message = "Category deactivated successfully",
                    StatusCode = HttpStatusCodes.OK
                };
            }
            else
            {
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found or already deactivated",
                    StatusCode = HttpStatusCodes.NotFound
                };
            }
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error deactivating category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> ReactivateAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var success = await _categoryRepository.ReactivateAsync(id);
            
            if (success)
            {
                return new JsonModel
                {
                    Success = true,
                    Message = "Category reactivated successfully",
                    StatusCode = HttpStatusCodes.OK
                };
            }
            else
            {
                return new JsonModel
                {
                    Success = false,
                    Message = "Category not found or already active",
                    StatusCode = HttpStatusCodes.NotFound
                };
            }
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error reactivating category: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetDeactivatedAsync(TokenModel tokenModel)
    {
        try
        {
            // TODO: Re-enable authentication for production
            // Validate user permissions
            // if (tokenModel.Role != "Admin")
            //     return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var categories = await _categoryRepository.GetDeactivatedAsync();
            
            return new JsonModel
            {
                Success = true,
                Data = categories,
                Message = "Deactivated categories retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving deactivated categories: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }
} 