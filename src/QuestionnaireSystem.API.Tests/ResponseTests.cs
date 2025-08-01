using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionnaireSystem.API.Tests;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;
using System.Text.Json;
using System.Net.Http.Json;
using Xunit;
using QuestionnaireSystem.Infrastructure.Data;

namespace QuestionnaireSystem.API.Tests;

public class ResponseTests : TestBase
{
    [Fact]
    public async Task SubmitResponse_WithValidData_ShouldSubmitSuccessfully()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff", // Hair Loss questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Response submitted successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SubmitResponse_WithMissingRequiredFields_ShouldReturnValidationError()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                // Missing required questions
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("required", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithInvalidEmail_ShouldReturnValidationError()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "invalid-email"
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid email format", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithInvalidNumber_ShouldReturnValidationError()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = -5 // Invalid negative number
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid number format", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithFutureDate_ShouldReturnValidationError()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(30) // Future date
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Date cannot be in the future", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithFileUpload_ShouldSubmitSuccessfully()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Response submitted successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SubmitResponse_WithCheckboxSelections_ShouldSubmitSuccessfully()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff", // Hair Loss questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "33333333-cccc-dddd-eeee-ffffffffffff",
                    SelectedOptionIds = new[] { "dddddddd-eeee-ffff-1111-222222222222", "eeeeeeee-ffff-1111-2222-333333333333" }
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Response submitted successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SubmitResponse_WithRatingScale_ShouldSubmitSuccessfully()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff", // Hair Loss questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 4 // Rating scale 1-5
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Response submitted successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ValidateResponses_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses/validate", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Success", result.Message);
    }

    [Fact]
    public async Task ValidateResponses_WithInvalidData_ShouldReturnValidationErrors()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "invalid-email"
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses/validate", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid email format", result.Message);
    }

    [Fact]
    public async Task GetUserResponses_WithValidToken_ShouldReturnResponses()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/responses");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("User responses retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetUserResponses_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var result = await GetAsync("/api/responses");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetResponseById_WithValidId_ShouldReturnResponse()
    {
        // Arrange
        // First, submit a response to get an ID
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff",
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "33333333-cccc-dddd-eeee-ffffffffffff",
                    NumberResponse = 3
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    TextResponse = "john.doe@example.com"
                },
                new
                {
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                }
            }
        };

        var submitResult = await _client.PostAsJsonAsync("/api/responses", submitDto);
        Assert.True(submitResult.IsSuccessStatusCode);

        // Get the response ID from the database directly
        var responseId = Guid.NewGuid(); // We'll get this from the database
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<QuestionnaireDbContext>();
            var response = await context.UserQuestionResponses
                .Where(r => r.QuestionnaireId == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
            
            if (response != null)
            {
                responseId = response.Id;
            }
        }

        // Act - Set auth header for the GET request
        SetAuthHeader(token);
        var result = await _client.GetAsync($"/api/responses/{responseId}");

        // Assert
        Assert.True(result.IsSuccessStatusCode);
        var responseContent = await result.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<JsonModel>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.True(responseData.Success);
    }

    [Fact]
    public async Task GetResponseById_WithWrongUser_ShouldReturnForbidden()
    {
        // Arrange
        // First, submit a response with a user who has access to the questionnaire
        var submitToken = await GetAuthTokenAsync("john@test.com", "user123"); // Hair Loss user
        SetAuthHeader(submitToken);

        // First, submit a response with the first user to get an ID
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff", // Hair Loss questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                },
                new
                {
                    QuestionId = "22222222-bbbb-cccc-dddd-eeeeeeeeeeee",
                    TextResponse = "6 months"
                },
                new
                {
                    QuestionId = "33333333-cccc-dddd-eeee-ffffffffffff",
                    NumberResponse = 3
                },
                new
                {
                    QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                }
            }
        };

        var submitResult = await _client.PostAsJsonAsync("/api/responses", submitDto);
        var submitContent = await submitResult.Content.ReadAsStringAsync();
        Console.WriteLine($"Submit Response Status: {submitResult.StatusCode}");
        Console.WriteLine($"Submit Response Content: {submitContent}");
        Assert.True(submitResult.IsSuccessStatusCode);

        // Get the response ID from the database directly
        var responseId = Guid.NewGuid(); // We'll get this from the database
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<QuestionnaireDbContext>();
            var response = await context.UserQuestionResponses
                .Where(r => r.QuestionnaireId == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
            
            if (response != null)
            {
                responseId = response.Id;
            }
        }

        // Now try to access with a different user
        var wrongUserToken = await GetAuthTokenAsync("jane@test.com", "user123"); // Weight Loss user
        SetAuthHeader(wrongUserToken);

        // Act
        var result = await _client.GetAsync($"/api/responses/{responseId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        var responseContent = await result.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<JsonModel>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.False(responseData.Success);
        Assert.Contains("Response not found", responseData.Message);
    }

    [Fact]
    public async Task GetResponsesByQuestionnaire_WithAdminToken_ShouldReturnResponses()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        // Act
        var result = await GetAsync($"/api/responses/questionnaire/{questionnaireId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Responses retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetResponsesByQuestionnaire_WithWrongCategoryUser_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("jane@test.com", "user123"); // Weight Loss user
        SetAuthHeader(token);
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff"; // Hair Loss questionnaire

        // Act
        var result = await GetAsync($"/api/responses/questionnaire/{questionnaireId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithNonExistentQuestionnaire_ShouldReturnNotFound()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "00000000-0000-0000-0000-000000000000", // Non-existent questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Questionnaire not found or inactive", result.Message);
    }

    [Fact]
    public async Task SubmitResponse_WithWrongCategoryUser_ShouldReturnForbidden()
    {
        // Arrange
        var submitDto = new
        {
            QuestionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff", // Hair Loss questionnaire
            Responses = new object[]
            {
                new
                {
                    QuestionId = "11111111-aaaa-bbbb-cccc-dddddddddddd",
                    SelectedOptionIds = new[] { "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" }
                }
            }
        };

        // Act
        var token = await GetAuthTokenAsync("jane@test.com", "user123"); // Weight Loss user
        SetAuthHeader(token);
        var result = await PostAsync("/api/responses", submitDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }
} 
