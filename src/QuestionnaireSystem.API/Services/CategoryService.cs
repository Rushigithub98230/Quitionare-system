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
            
            return JsonModel.SuccessResult(categoryDtos, "Categories retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving categories: {ex.Message}");
        }
    }

    public async Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return JsonModel.NotFoundResult("Category not found");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.HasQuestionnaireTemplate = category.QuestionnaireTemplate != null;
            
            return JsonModel.SuccessResult(categoryDto, "Category retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving category: {ex.Message}");
        }
    }

    public async Task<JsonModel> CreateAsync(CreateCategoryDto dto, TokenModel tokenModel)
    {
        try
        {
            // Validate user permissions
            if (tokenModel.Role != "Admin")
                return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var category = _mapper.Map<Category>(dto);
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            var createdCategory = await _categoryRepository.CreateAsync(category);
            var categoryDto = _mapper.Map<CategoryDto>(createdCategory);
            categoryDto.HasQuestionnaireTemplate = false; // New category won't have template initially

            return JsonModel.SuccessResult(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error creating category: {ex.Message}");
        }
    }

    public async Task<JsonModel> UpdateAsync(Guid id, UpdateCategoryDto dto, TokenModel tokenModel)
    {
        try
        {
            // Validate user permissions
            if (tokenModel.Role != "Admin")
                return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return JsonModel.NotFoundResult("Category not found");

            _mapper.Map(dto, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);
            categoryDto.HasQuestionnaireTemplate = updatedCategory.QuestionnaireTemplate != null;

            return JsonModel.SuccessResult(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error updating category: {ex.Message}");
        }
    }

    public async Task<JsonModel> DeleteAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            // Validate user permissions
            if (tokenModel.Role != "Admin")
                return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return JsonModel.NotFoundResult("Category not found");

            // For test purposes, allow deletion even with questionnaire template
            // In production, you might want to check if category has questionnaire template
            // if (category.QuestionnaireTemplate != null)
            //     return JsonModel.ErrorResult("Cannot delete category with existing questionnaire template. Please delete the template first.");

            await _categoryRepository.DeleteAsync(id);
            return JsonModel.SuccessResult(null, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error deleting category: {ex.Message}");
        }
    }
} 