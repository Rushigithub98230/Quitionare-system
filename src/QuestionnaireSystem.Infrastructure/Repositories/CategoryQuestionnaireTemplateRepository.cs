using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.Infrastructure.Repositories;

public class CategoryQuestionnaireTemplateRepository : ICategoryQuestionnaireTemplateRepository
{
    private readonly QuestionnaireDbContext _context;

    public CategoryQuestionnaireTemplateRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryQuestionnaireTemplate?> GetByIdAsync(Guid id)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<CategoryQuestionnaireTemplate?> GetByIdWithQuestionsAsync(Guid id)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.QuestionType)
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.Options.OrderBy(option => option.DisplayOrder))
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<CategoryQuestionnaireTemplate?> GetByIdWithCategoryAsync(Guid id)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.QuestionType)
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.Options.OrderBy(option => option.DisplayOrder))
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<CategoryQuestionnaireTemplate?> GetByIdWithResponsesAsync(Guid id)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.QuestionType)
            .Include(q => q.Questions.OrderBy(question => question.DisplayOrder))
                .ThenInclude(question => question.Options.OrderBy(option => option.DisplayOrder))
            .Include(q => q.UserResponses)
                .ThenInclude(response => response.User)
            .Include(q => q.UserResponses)
                .ThenInclude(response => response.QuestionResponses.OrderBy(r => r.Question.DisplayOrder))
                .ThenInclude(response => response.Question)
            .Include(q => q.UserResponses)
                .ThenInclude(response => response.QuestionResponses.OrderBy(r => r.Question.DisplayOrder))
                .ThenInclude(response => response.OptionResponses)
            .FirstOrDefaultAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplate>> GetAllAsync()
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Where(q => q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplate>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Where(q => q.CategoryId == categoryId && q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplate>> GetActiveAsync()
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Where(q => q.IsActive && q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplate>> GetByUserIdAsync(int userId)
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Where(q => q.CreatedBy == userId && q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<CategoryQuestionnaireTemplate> CreateAsync(CategoryQuestionnaireTemplate questionnaire)
    {
        _context.CategoryQuestionnaireTemplates.Add(questionnaire);
        await _context.SaveChangesAsync();
        
        return questionnaire;
    }

    public async Task<CategoryQuestionnaireTemplate> UpdateAsync(CategoryQuestionnaireTemplate questionnaire)
    {
        _context.CategoryQuestionnaireTemplates.Update(questionnaire);
        await _context.SaveChangesAsync();
        
        return questionnaire;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var questionnaire = await GetByIdAsync(id);
        if (questionnaire == null)
            return false;

        questionnaire.DeletedDate = DateTime.UtcNow;
        questionnaire.IsDeleted = true;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.CategoryQuestionnaireTemplates
            .AnyAsync(q => q.Id == id && q.DeletedDate == null);
    }

    public async Task<int> GetCountByCategoryIdAsync(Guid categoryId)
    {
        return await _context.CategoryQuestionnaireTemplates
            .CountAsync(q => q.CategoryId == categoryId);
    }

    public async Task<bool> HasResponsesAsync(Guid questionnaireId)
    {
        return await _context.UserQuestionResponses
            .AnyAsync(r => r.QuestionnaireId == questionnaireId);
    }

    public async Task<bool> TitleExistsAsync(string title, Guid? excludeId = null)
    {
        var query = _context.CategoryQuestionnaireTemplates
            .Where(q => q.Title == title && q.DeletedDate == null);
        
        if (excludeId.HasValue)
        {
            query = query.Where(q => q.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<CategoryQuestionnaireTemplateSummaryDto>> GetSummaryAsync()
    {
        return await _context.CategoryQuestionnaireTemplates
            .Include(q => q.Category)
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .Include(q => q.UserResponses)
            .Where(q => q.DeletedDate == null)
            .OrderBy(q => q.DisplayOrder)
            .ThenBy(q => q.Title)
            .Select(q => new CategoryQuestionnaireTemplateSummaryDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                CategoryId = q.CategoryId,
                IsActive = q.IsActive,
                IsMandatory = q.IsMandatory,
                DisplayOrder = q.DisplayOrder,
                Version = q.Version,
                CreatedBy = q.CreatedBy,
                CreatedDate = q.CreatedDate,
                UpdatedDate = q.UpdatedDate,
                DeletedDate = q.DeletedDate,
                Category = q.Category != null ? new CategoryDto
                {
                    Id = q.Category.Id,
                    Name = q.Category.Name,
                    Description = q.Category.Description,
                    Icon = q.Category.Icon,
                    Color = q.Category.Color,
                    IsActive = q.Category.IsActive,
                    DisplayOrder = q.Category.DisplayOrder,
                    Features = q.Category.Features,
                    ConsultationDescription = q.Category.ConsultationDescription,
                    BasePrice = q.Category.BasePrice,
                    RequiresQuestionnaireAssessment = q.Category.RequiresQuestionnaireAssessment,
                    AllowsMedicationDelivery = q.Category.AllowsMedicationDelivery,
                    AllowsFollowUpMessaging = q.Category.AllowsFollowUpMessaging,
                    OneTimeConsultationDurationMinutes = q.Category.OneTimeConsultationDurationMinutes,
                    IsMostPopular = q.Category.IsMostPopular,
                    IsTrending = q.Category.IsTrending,
                    CreatedDate = q.Category.CreatedDate,
                    UpdatedDate = q.Category.UpdatedDate
                } : null,
                CreatedByUser = q.CreatedByUser != null ? new UserDto
                {
                    Id = q.CreatedByUser.Id,
                    Email = q.CreatedByUser.Email,
                    FirstName = q.CreatedByUser.FirstName,
                    LastName = q.CreatedByUser.LastName,
                    Role = q.CreatedByUser.Role,
                    Category = q.CreatedByUser.Category,
                    CreatedDate = q.CreatedByUser.CreatedDate
                } : null,
                QuestionCount = q.Questions.Count(question => question.DeletedDate == null),
                ResponseCount = q.UserResponses.Count()
            })
            .ToListAsync();
    }
} 