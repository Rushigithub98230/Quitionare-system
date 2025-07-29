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
            .ForMember(dest => dest.QuestionnaireCount, opt => opt.MapFrom(src => src.Questionnaires.Count));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Questionnaire mappings
        CreateMap<QuestionnaireTemplate, QuestionnaireDetailDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<QuestionnaireTemplate, QuestionnaireResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<CreateQuestionnaireDto, QuestionnaireTemplate>();
        CreateMap<UpdateQuestionnaireDto, QuestionnaireTemplate>();

        // Question mappings
        CreateMap<Question, QuestionDetailDto>()
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.QuestionType.TypeName))
            .ForMember(dest => dest.QuestionTypeDisplayName, opt => opt.MapFrom(src => src.QuestionType.DisplayName))
            .ForMember(dest => dest.ValidationRules, opt => opt.Ignore())
            .ForMember(dest => dest.ConditionalLogic, opt => opt.Ignore())
            .ForMember(dest => dest.Settings, opt => opt.Ignore());

        CreateMap<CreateQuestionDto, Question>();
        CreateMap<UpdateQuestionDto, Question>();

        // Question option mappings
        CreateMap<QuestionOption, QuestionOptionDetailDto>();
        CreateMap<CreateQuestionOptionDto, QuestionOption>();

        // Patient response mappings
        CreateMap<PatientResponse, ResponseSummaryDto>()
            .ForMember(dest => dest.QuestionnaireTitle, opt => opt.MapFrom(src => src.Questionnaire.Title))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Questionnaire.Category.Name));

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();
    }
} 