using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Infrastructure.Data;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class CategoryQuestionRepository : ICategoryQuestionRepository
{
    private readonly QuestionnaireDbContext _context;

    public CategoryQuestionRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryQuestion?> GetByIdAsync(Guid id)
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<IEnumerable<CategoryQuestion>> GetByQuestionnaireIdAsync(Guid questionnaireId)
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .Where(q => q.QuestionnaireId == questionnaireId && q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryQuestion>> GetAllAsync()
    {
        return await _context.CategoryQuestions
            .Include(q => q.QuestionType)
            .Include(q => q.Options.OrderBy(option => option.DisplayOrder))
            .Where(q => q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ToListAsync();
    }

    public async Task<CategoryQuestion> CreateAsync(CategoryQuestion question)
    {
        // Add the question (without options to avoid duplication)
        _context.CategoryQuestions.Add(question);
        await _context.SaveChangesAsync();
        
        return question;
    }

    public async Task<CategoryQuestion> UpdateAsync(CategoryQuestion question)
    {
        // Detach any existing tracked entity to avoid concurrency issues
        var existingEntity = _context.CategoryQuestions.Local.FirstOrDefault(e => e.Id == question.Id);
        if (existingEntity != null)
        {
            _context.Entry(existingEntity).State = EntityState.Detached;
        }
        
        // Use Update method which will handle the entity properly
        _context.CategoryQuestions.Update(question);
        
        await _context.SaveChangesAsync();
        
        return question;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var question = await GetByIdAsync(id);
        if (question == null)
            return false;

        question.DeletedDate = DateTime.UtcNow;
        question.IsDeleted = true;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.CategoryQuestions
            .AnyAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<bool> HasResponsesAsync(Guid questionId)
    {
        return await _context.QuestionResponses
            .AnyAsync(r => r.QuestionId == questionId);
    }

    public async Task<QuestionType?> GetQuestionTypeByIdAsync(Guid questionTypeId)
    {
        return await _context.QuestionTypes
            .FirstOrDefaultAsync(qt => qt.Id == questionTypeId);
    }

    public async Task<int> GetMaxDisplayOrderAsync(Guid questionnaireId)
    {
        var maxOrder = await _context.CategoryQuestions
            .Where(q => q.QuestionnaireId == questionnaireId && q.DeletedDate == null)
            .MaxAsync(q => (int?)q.DisplayOrder);
        
        return maxOrder ?? 0;
    }

    public async Task UpdateQuestionOptionsAsync(Guid questionId, IEnumerable<UpdateQuestionOptionDto> options)
    {
        // Clear any existing tracked entities for this question to avoid conflicts
        var trackedOptions = _context.QuestionOptions.Local.Where(o => o.QuestionId == questionId).ToList();
        foreach (var trackedOption in trackedOptions)
        {
            _context.Entry(trackedOption).State = EntityState.Detached;
        }
        
        // First, delete existing options for this question
        var existingOptions = await _context.QuestionOptions
            .Where(o => o.QuestionId == questionId)
            .ToListAsync();
        
        _context.QuestionOptions.RemoveRange(existingOptions);
        await _context.SaveChangesAsync();
        
        // Add new options
        foreach (var optionDto in options)
        {
            var option = new QuestionOption
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                OptionText = optionDto.OptionText,
                OptionValue = optionDto.OptionValue,
                DisplayOrder = optionDto.DisplayOrder,
                IsCorrect = optionDto.IsCorrect,
                IsActive = optionDto.IsActive
            };
            
            _context.QuestionOptions.Add(option);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task ClearQuestionOptionsAsync(Guid questionId)
    {
        // Delete all existing options for this question
        var existingOptions = await _context.QuestionOptions
            .Where(o => o.QuestionId == questionId)
            .ToListAsync();
        
        _context.QuestionOptions.RemoveRange(existingOptions);
        await _context.SaveChangesAsync();
    }

    // Question Option management methods
    public async Task<QuestionOption> CreateOptionAsync(QuestionOption option)
    {
        // Check if option already exists to prevent duplicates
        var existingOption = await _context.QuestionOptions
            .FirstOrDefaultAsync(o => o.QuestionId == option.QuestionId && 
                                    o.OptionText == option.OptionText && 
                                    o.OptionValue == option.OptionValue);
        
        if (existingOption != null)
        {
            return existingOption; // Return existing option instead of creating duplicate
        }
        
        _context.QuestionOptions.Add(option);
        await _context.SaveChangesAsync();
        
        return option;
    }

    public async Task<IEnumerable<QuestionOption>> GetOptionsByQuestionIdAsync(Guid questionId)
    {
        return await _context.QuestionOptions
            .Where(o => o.QuestionId == questionId)
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync();
    }

    public async Task<QuestionOption?> GetOptionByIdAsync(Guid optionId)
    {
        return await _context.QuestionOptions
            .FirstOrDefaultAsync(o => o.Id == optionId);
    }

    public async Task<QuestionOption> UpdateOptionAsync(QuestionOption option)
    {
        // Check if the entity is already being tracked
        var existingEntity = _context.QuestionOptions.Local.FirstOrDefault(e => e.Id == option.Id);
        if (existingEntity != null)
        {
            // Update the existing tracked entity
            _context.Entry(existingEntity).CurrentValues.SetValues(option);
        }
        else
        {
            // Use Update method for untracked entities
            _context.QuestionOptions.Update(option);
        }
        
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task<bool> DeleteOptionAsync(Guid optionId)
    {
        var option = await GetOptionByIdAsync(optionId);
        if (option == null)
            return false;

        _context.QuestionOptions.Remove(option);
        await _context.SaveChangesAsync();
        
        return true;
    }
} 