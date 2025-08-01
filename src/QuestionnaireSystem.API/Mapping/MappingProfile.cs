using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Models;
using System.Text.Json;

namespace QuestionnaireSystem.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.HasQuestionnaireTemplate, opt => opt.MapFrom(src => src.QuestionnaireTemplate != null));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Questionnaire template mappings
        CreateMap<CategoryQuestionnaireTemplate, CategoryQuestionnaireTemplateDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.FirstName + " " + src.CreatedByUser.LastName))
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count));

        CreateMap<CategoryQuestionnaireTemplate, CategoryQuestionnaireTemplateSummaryDto>()
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
            .ForMember(dest => dest.ResponseCount, opt => opt.MapFrom(src => src.UserResponses.Count));

        CreateMap<CategoryQuestionnaireTemplate, CategoryQuestionnaireTemplateWithCategoryDto>();

        CreateMap<CategoryQuestionnaireTemplate, CategoryQuestionnaireTemplateWithResponsesDto>();

        CreateMap<CreateCategoryQuestionnaireTemplateDto, CategoryQuestionnaireTemplate>();
        CreateMap<UpdateCategoryQuestionnaireTemplateDto, CategoryQuestionnaireTemplate>();

        // Question mappings
        CreateMap<CategoryQuestion, CategoryQuestionDto>()
            .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.QuestionType.DisplayName));

        CreateMap<CreateCategoryQuestionDto, CategoryQuestion>();
        CreateMap<UpdateCategoryQuestionDto, CategoryQuestion>();

        // Question option mappings
        CreateMap<QuestionOption, QuestionOptionDto>();
        CreateMap<CreateQuestionOptionDto, QuestionOption>();
        CreateMap<UpdateQuestionOptionDto, QuestionOption>();

        // User question response mappings
        CreateMap<UserQuestionResponse, ResponseSummaryDto>()
            .ForMember(dest => dest.QuestionnaireTitle, opt => opt.MapFrom(src => src.Questionnaire.Title))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Questionnaire.Category.Name));

        // Question type mappings
        CreateMap<QuestionType, QuestionTypeDto>();

        CreateMap<UserQuestionResponse, ResponseDetailDto>()
            .ForMember(dest => dest.QuestionnaireTitle, opt => opt.MapFrom(src => src.Questionnaire.Title))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Questionnaire.Category.Name));

        CreateMap<QuestionResponse, QuestionResponseDetailDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Question.QuestionType.TypeName));

        CreateMap<QuestionOptionResponse, QuestionOptionResponseDetailDto>()
            .ForMember(dest => dest.OptionText, opt => opt.MapFrom(src => src.Option.OptionText))
            .ForMember(dest => dest.OptionValue, opt => opt.MapFrom(src => src.Option.OptionValue));

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();
    }
} 