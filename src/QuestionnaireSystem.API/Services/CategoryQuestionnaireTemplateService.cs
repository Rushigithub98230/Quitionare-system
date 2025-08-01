using AutoMapper;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Services;

public class CategoryQuestionnaireTemplateService : ICategoryQuestionnaireTemplateService
{
    private readonly ICategoryQuestionnaireTemplateRepository _questionnaireRepository;
    private readonly ICategoryQuestionRepository _questionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryQuestionnaireTemplateService(
        ICategoryQuestionnaireTemplateRepository questionnaireRepository,
        ICategoryQuestionRepository questionRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _questionnaireRepository = questionnaireRepository;
        _questionRepository = questionRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryQuestionnaireTemplateDto?> GetByIdAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(id);
        if (questionnaire == null)
            return null;

        return _mapper.Map<CategoryQuestionnaireTemplateDto>(questionnaire);
    }

    public async Task<CategoryQuestionnaireTemplateWithCategoryDto?> GetByIdWithCategoryAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithCategoryAsync(id);
        if (questionnaire == null)
            return null;

        return _mapper.Map<CategoryQuestionnaireTemplateWithCategoryDto>(questionnaire);
    }

    public async Task<CategoryQuestionnaireTemplateWithResponsesDto?> GetByIdWithResponsesAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithResponsesAsync(id);
        if (questionnaire == null)
            return null;

        return _mapper.Map<CategoryQuestionnaireTemplateWithResponsesDto>(questionnaire);
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetAllAsync()
    {
        var questionnaires = await _questionnaireRepository.GetSummaryAsync();
        return questionnaires;
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetByCategoryIdAsync(Guid categoryId)
    {
        var questionnaires = await _questionnaireRepository.GetByCategoryIdAsync(categoryId);
        return _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetActiveAsync()
    {
        var questionnaires = await _questionnaireRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetByUserIdAsync(Guid userId)
    {
        var questionnaires = await _questionnaireRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>>(questionnaires);
    }

    public async Task<CategoryQuestionnaireTemplateDto> CreateAsync(CreateCategoryQuestionnaireTemplateDto createDto)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        // Validate questions first (before checking for existing questionnaires)
        if (createDto.Questions != null && createDto.Questions.Any())
        {
            // Check for duplicate display orders
            var displayOrders = createDto.Questions.Select(q => q.DisplayOrder).ToList();
            var duplicateOrders = displayOrders.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateOrders.Any())
                throw new ArgumentException($"Duplicate question order: {string.Join(", ", duplicateOrders)}");

            foreach (var questionDto in createDto.Questions)
            {
                // Validate question type exists
                var questionType = await _questionRepository.GetQuestionTypeByIdAsync(questionDto.QuestionTypeId);
                if (questionType == null)
                    throw new ArgumentException("Invalid question type");
            }
        }

        // Check if questionnaire already exists for this category
        var existingQuestionnaires = await _questionnaireRepository.GetByCategoryIdAsync(createDto.CategoryId);
        if (existingQuestionnaires.Any())
            throw new ArgumentException("Questionnaire template already exists for this category");

        // Create questionnaire
        var questionnaire = _mapper.Map<CategoryQuestionnaireTemplate>(createDto);
        questionnaire.CreatedAt = DateTime.UtcNow;
        questionnaire.UpdatedAt = DateTime.UtcNow;
        
        questionnaire = await _questionnaireRepository.CreateAsync(questionnaire);

        // Create questions
        if (createDto.Questions != null && createDto.Questions.Any())
        {
            foreach (var questionDto in createDto.Questions)
            {
                var question = _mapper.Map<CategoryQuestion>(questionDto);
                question.QuestionnaireId = questionnaire.Id;
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.CreateAsync(question);
            }
        }

        // Return the created questionnaire
        var createdQuestionnaire = await _questionnaireRepository.GetByIdAsync(questionnaire.Id);
        if (createdQuestionnaire == null)
            throw new InvalidOperationException("Failed to retrieve created questionnaire");
            
        return _mapper.Map<CategoryQuestionnaireTemplateDto>(createdQuestionnaire);
    }

    public async Task<CategoryQuestionnaireTemplateDto?> UpdateAsync(Guid id, UpdateCategoryQuestionnaireTemplateDto updateDto)
    {
        var existingQuestionnaire = await _questionnaireRepository.GetByIdAsync(id);
        if (existingQuestionnaire == null)
            return null;

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(updateDto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        // Update questionnaire properties
        _mapper.Map(updateDto, existingQuestionnaire);
        existingQuestionnaire.UpdatedAt = DateTime.UtcNow;
        existingQuestionnaire = await _questionnaireRepository.UpdateAsync(existingQuestionnaire);

        // Update questions
        if (updateDto.Questions != null && updateDto.Questions.Any())
        {
            // Delete existing questions
            var existingQuestions = await _questionRepository.GetByQuestionnaireIdAsync(id);
            foreach (var question in existingQuestions)
            {
                await _questionRepository.DeleteAsync(question.Id);
            }

            // Create new questions
            foreach (var questionDto in updateDto.Questions)
            {
                var question = _mapper.Map<CategoryQuestion>(questionDto);
                question.QuestionnaireId = id;
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.CreateAsync(question);
            }
        }

        // Return the updated questionnaire
        var updatedQuestionnaire = await _questionnaireRepository.GetByIdAsync(id);
        return _mapper.Map<CategoryQuestionnaireTemplateDto>(updatedQuestionnaire);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // Check if questionnaire has responses
        var hasResponses = await _questionnaireRepository.HasResponsesAsync(id);
        if (hasResponses)
            return false; // Cannot delete questionnaire with responses

        return await _questionnaireRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _questionnaireRepository.ExistsAsync(id);
    }

    public async Task<int> GetCountByCategoryIdAsync(Guid categoryId)
    {
        return await _questionnaireRepository.GetCountByCategoryIdAsync(categoryId);
    }

    public async Task<bool> HasResponsesAsync(Guid questionnaireId)
    {
        return await _questionnaireRepository.HasResponsesAsync(questionnaireId);
    }
} 