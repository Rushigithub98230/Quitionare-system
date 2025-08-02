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
            
            return new JsonModel
            {
                Success = true,
                Data = questionTypeDtos,
                Message = "Question types retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving question types: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetByIdAsync(Guid id, TokenModel tokenModel)
    {
        try
        {
            var questionType = await _questionTypeRepository.GetByIdAsync(id);
            if (questionType == null)
                return new JsonModel
                {
                    Success = false,
                    Message = "Question type not found",
                    StatusCode = HttpStatusCodes.NotFound
                };

            var questionTypeDto = _mapper.Map<QuestionTypeDto>(questionType);
            return new JsonModel
            {
                Success = true,
                Data = questionTypeDto,
                Message = "Question type retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving question type: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }

    public async Task<JsonModel> GetActiveAsync(TokenModel tokenModel)
    {
        try
        {
            var questionTypes = await _questionTypeRepository.GetActiveAsync();
            var questionTypeDtos = _mapper.Map<List<QuestionTypeDto>>(questionTypes);
            
            return new JsonModel
            {
                Success = true,
                Data = questionTypeDtos,
                Message = "Active question types retrieved successfully",
                StatusCode = HttpStatusCodes.OK
            };
        }
        catch (Exception ex)
        {
            return new JsonModel
            {
                Success = false,
                Message = $"Error retrieving active question types: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            };
        }
    }
} 