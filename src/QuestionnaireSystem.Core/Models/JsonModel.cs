using System.Net;

namespace QuestionnaireSystem.Core.Models;

public class JsonModel
{
    public object? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static JsonModel SuccessResult(object? data, string message = "Success")
    {
        return new JsonModel
        {
            Data = data,
            Message = message,
            StatusCode = HttpStatusCodes.OK,
            Success = true
        };
    }

    public static JsonModel ErrorResult(string message, int statusCode = HttpStatusCodes.BadRequest)
    {
        return new JsonModel
        {
            Message = message,
            StatusCode = statusCode,
            Success = false
        };
    }

    public static JsonModel NotFoundResult(string message = "Resource not found")
    {
        return new JsonModel
        {
            Message = message,
            StatusCode = HttpStatusCodes.NotFound,
            Success = false
        };
    }

    public static JsonModel ValidationErrorResult(string message)
    {
        return new JsonModel
        {
            Message = message,
            StatusCode = HttpStatusCodes.BadRequest,
            Success = false
        };
    }
}

public static class HttpStatusCodes
{
    public const int OK = 200;
    public const int Created = 201;
    public const int NoContent = 204;
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int InternalServerError = 500;
    public const int NotImplemented = 501;
} 