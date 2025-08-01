using AutoMapper;
using QuestionnaireSystem.Core.DTOs;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.API.Services;

public class QuestionTypeService : IQuestionTypeService
{
    private readonly IQuestionTypeRepository _questionTypeRepository;
    private readonly IMapper _mapper;

    public QuestionTypeService(IQuestionTypeRepository questionTypeRepository, IMapper mapper)
    {
        _questionTypeRepository = questionTypeRepository;
        _mapper = mapper;
    }

    public async Task<JsonModel> GetAllAsync(TokenModel tokenModel)
    {
        try
        {
            var questionTypes = await _questionTypeRepository.GetAllAsync();
            var questionTypeDtos = _mapper.Map<List<QuestionTypeDto>>(questionTypes);
            
            return JsonModel.SuccessResult(questionTypeDtos, "Question types retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving question types: {ex.Message}");
        }
    }

    public async Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            var questionType = await _questionTypeRepository.GetByIdAsync(id);
            if (questionType == null)
                return JsonModel.NotFoundResult("Question type not found");

            var questionTypeDto = _mapper.Map<QuestionTypeDto>(questionType);
            return JsonModel.SuccessResult(questionTypeDto, "Question type retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving question type: {ex.Message}");
        }
    }

    public async Task<JsonModel> GetActiveAsync(TokenModel tokenModel)
    {
        try
        {
            var questionTypes = await _questionTypeRepository.GetActiveAsync();
            var questionTypeDtos = _mapper.Map<List<QuestionTypeDto>>(questionTypes);
            
            return JsonModel.SuccessResult(questionTypeDtos, "Active question types retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving active question types: {ex.Message}");
        }
    }
} 