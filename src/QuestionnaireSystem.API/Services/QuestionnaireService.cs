using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using System.Text.Json;

namespace QuestionnaireSystem.API.Services;

public class QuestionnaireService : IQuestionnaireService
{
    private readonly IQuestionnaireRepository _questionnaireRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public QuestionnaireService(
        IQuestionnaireRepository questionnaireRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _questionnaireRepository = questionnaireRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<QuestionnaireDetailDto?> GetByIdAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetDetailsByIdAsync(id);
        return _mapper.Map<QuestionnaireDetailDto>(questionnaire);
    }

    public async Task<IEnumerable<QuestionnaireDetailDto>> GetAllAsync(bool includeInactive = false)
    {
        var questionnaires = await _questionnaireRepository.GetAllAsync(includeInactive);
        return _mapper.Map<IEnumerable<QuestionnaireDetailDto>>(questionnaires);
    }

    public async Task<IEnumerable<QuestionnaireDetailDto>> GetByCategoryAsync(Guid categoryId)
    {
        var questionnaires = await _questionnaireRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<QuestionnaireDetailDto>>(questionnaires);
    }

    public async Task<QuestionnaireDetailDto> CreateAsync(CreateQuestionnaireDto dto, Guid createdBy)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        var questionnaire = _mapper.Map<QuestionnaireTemplate>(dto);
        questionnaire.CreatedBy = createdBy;
        questionnaire.CreatedAt = DateTime.UtcNow;
        questionnaire.UpdatedAt = DateTime.UtcNow;

        // Process questions
        foreach (var questionDto in dto.Questions)
        {
            var question = _mapper.Map<Question>(questionDto);
            question.QuestionnaireId = questionnaire.Id;
            question.CreatedAt = DateTime.UtcNow;
            question.UpdatedAt = DateTime.UtcNow;

            // Process validation rules and settings
            if (questionDto.ValidationRules != null)
                question.ValidationRules = JsonSerializer.Serialize(questionDto.ValidationRules);
            if (questionDto.ConditionalLogic != null)
                question.ConditionalLogic = JsonSerializer.Serialize(questionDto.ConditionalLogic);
            if (questionDto.Settings != null)
                question.Settings = JsonSerializer.Serialize(questionDto.Settings);

            // Process options
            foreach (var optionDto in questionDto.Options)
            {
                var option = _mapper.Map<QuestionOption>(optionDto);
                option.QuestionId = question.Id;
                option.CreatedAt = DateTime.UtcNow;
                question.Options.Add(option);
            }

            questionnaire.Questions.Add(question);
        }

        var createdQuestionnaire = await _questionnaireRepository.CreateAsync(questionnaire);
        return _mapper.Map<QuestionnaireDetailDto>(createdQuestionnaire);
    }

    public async Task<QuestionnaireDetailDto> UpdateAsync(Guid id, UpdateQuestionnaireDto dto)
    {
        var existingQuestionnaire = await _questionnaireRepository.GetDetailsByIdAsync(id);
        if (existingQuestionnaire == null)
            throw new ArgumentException("Questionnaire not found");

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        // Update basic properties
        existingQuestionnaire.Title = dto.Title;
        existingQuestionnaire.Description = dto.Description;
        existingQuestionnaire.CategoryId = dto.CategoryId;
        existingQuestionnaire.IsActive = dto.IsActive;
        existingQuestionnaire.IsMandatory = dto.IsMandatory;
        existingQuestionnaire.DisplayOrder = dto.DisplayOrder;
        existingQuestionnaire.UpdatedAt = DateTime.UtcNow;

        // Increment version for tracking
        existingQuestionnaire.Version = await _questionnaireRepository.GetNextVersionAsync(id);

        // Process questions (simplified - in production you'd want more sophisticated update logic)
        existingQuestionnaire.Questions.Clear();
        foreach (var questionDto in dto.Questions)
        {
            var question = _mapper.Map<Question>(questionDto);
            question.QuestionnaireId = existingQuestionnaire.Id;
            question.CreatedAt = DateTime.UtcNow;
            question.UpdatedAt = DateTime.UtcNow;

            // Process validation rules and settings
            if (questionDto.ValidationRules != null)
                question.ValidationRules = JsonSerializer.Serialize(questionDto.ValidationRules);
            if (questionDto.ConditionalLogic != null)
                question.ConditionalLogic = JsonSerializer.Serialize(questionDto.ConditionalLogic);
            if (questionDto.Settings != null)
                question.Settings = JsonSerializer.Serialize(questionDto.Settings);

            // Process options
            foreach (var optionDto in questionDto.Options)
            {
                var option = _mapper.Map<QuestionOption>(optionDto);
                option.QuestionId = question.Id;
                option.CreatedAt = DateTime.UtcNow;
                question.Options.Add(option);
            }

            existingQuestionnaire.Questions.Add(question);
        }

        var updatedQuestionnaire = await _questionnaireRepository.UpdateAsync(existingQuestionnaire);
        return _mapper.Map<QuestionnaireDetailDto>(updatedQuestionnaire);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _questionnaireRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _questionnaireRepository.ExistsAsync(id);
    }

    public async Task<QuestionnaireResponseDto?> GetForResponseAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetDetailsByIdAsync(id);
        if (questionnaire == null || !questionnaire.IsActive)
            return null;

        return _mapper.Map<QuestionnaireResponseDto>(questionnaire);
    }
} 