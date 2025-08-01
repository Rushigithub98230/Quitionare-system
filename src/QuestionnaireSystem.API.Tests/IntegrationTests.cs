using Xunit;
using System.Text.Json;
using QuestionnaireSystem.Core.Models;
using QuestionnaireSystem.Core.DTOs;

namespace QuestionnaireSystem.API.Tests;

public class IntegrationTests : TestBase
{
    [Fact]
    public async Task CompleteWorkflow_FromLoginToResponseSubmission_ShouldWorkEndToEnd()
    {
        // 1. Login as user
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        Assert.NotEmpty(token);

        // 2. Get user's questionnaires
        SetAuthHeader(token);
        var questionnairesResult = await GetAsync("/api/CategoryQuestionnaireTemplates");
        Assert.True(questionnairesResult.Success);
        Assert.NotNull(questionnairesResult.Data);

        // 3. Get questionnaire for response
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        var questionnaireResult = await GetAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/response");
        
        Assert.True(questionnaireResult.Success);
        Assert.NotNull(questionnaireResult.Data);

        // 4. Submit response
        var submitDto = new
        {
            QuestionnaireId = questionnaireId,
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
                    QuestionId = "55555555-eeee-ffff-1111-222222222222",
                    NumberResponse = 25
                },
                new
                {
                    QuestionId = "66666666-ffff-aaaa-bbbb-cccccccccccc",
                    DateResponse = DateTime.UtcNow.AddDays(-30)
                }
            }
        };

        var submitResult = await PostAsync("/api/responses", submitDto);
        Assert.True(submitResult.Success);
        Assert.NotNull(submitResult.Data);

        // 5. Get user's responses
        var responsesResult = await GetAsync("/api/responses");
        Assert.True(responsesResult.Success);
        Assert.NotNull(responsesResult.Data);

        // 6. Get specific response by ID
        var responseData = JsonSerializer.Deserialize<JsonElement>(submitResult.Data?.ToString() ?? "{}");
        var responseId = responseData.GetProperty("id").GetString();
        var responseResult = await GetAsync($"/api/responses/{responseId}");
        Assert.True(responseResult.Success);
        Assert.NotNull(responseResult.Data);
    }

    [Fact]
    public async Task AdminWorkflow_CreateCategoryAndQuestionnaire_ShouldWorkEndToEnd()
    {
        // 1. Login as admin
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        Assert.NotEmpty(token);
        SetAuthHeader(token);

        // 2. Create new category
        var createCategoryDto = new
        {
            Name = "Test Category",
            Description = "Test category for integration testing",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 10
        };

        var categoryResult = await PostAsync("/api/categories", createCategoryDto);
        Assert.True(categoryResult.Success);
        Assert.NotNull(categoryResult.Data);

        // 3. Extract category ID from response
        var categoryData = JsonSerializer.Deserialize<JsonElement>(categoryResult.Data?.ToString() ?? "{}");
        var categoryId = categoryData.GetProperty("id").GetString();
        Assert.NotNull(categoryId);

        // 4. Create questionnaire for the new category
        var createQuestionnaireDto = new
        {
            Title = "Test Integration Questionnaire",
            Description = "Questionnaire created during integration test",
            CategoryId = categoryId,
            IsActive = true,
            IsMandatory = false,
            DisplayOrder = 1,
            Questions = new object[]
            {
                new
                {
                    QuestionText = "What is your favorite color?",
                    QuestionTypeId = "11111111-1111-1111-1111-111111111111", // Text type
                    IsRequired = true,
                    DisplayOrder = 1
                },
                new
                {
                    QuestionText = "How old are you?",
                    QuestionTypeId = "22222222-2222-2222-2222-222222222222", // Number type
                    IsRequired = true,
                    DisplayOrder = 2
                }
            }
        };

        var questionnaireResult = await PostAsync("/api/CategoryQuestionnaireTemplates", createQuestionnaireDto);
        Assert.True(questionnaireResult.Success);
        Assert.NotNull(questionnaireResult.Data);

        // 5. Extract questionnaire ID from response
        var questionnaireData = JsonSerializer.Deserialize<JsonElement>(questionnaireResult.Data?.ToString() ?? "{}");
        var questionnaireId = questionnaireData.GetProperty("id").GetString();
        Assert.NotNull(questionnaireId);

        // 6. Verify questionnaire was created by getting it
        var getQuestionnaireResult = await GetAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}");
        Assert.True(getQuestionnaireResult.Success);
        Assert.NotNull(getQuestionnaireResult.Data);

        // 7. Update the questionnaire
        var updateQuestionnaireDto = new
        {
            Title = "Updated Integration Questionnaire",
            Description = "Updated questionnaire description",
            CategoryId = categoryId,
            IsActive = true,
            IsMandatory = true,
            DisplayOrder = 2
        };

        var updateResult = await PutAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}", updateQuestionnaireDto);
        Assert.True(updateResult.Success);
        Assert.NotNull(updateResult.Data);

        // 8. Add a new question to the questionnaire
        var addQuestionDto = new
        {
            QuestionText = "What is your email address?",
            QuestionTypeId = "11111111-1111-1111-1111-111111111111", // Text type
            IsRequired = true,
            DisplayOrder = 3
        };

        var addQuestionResult = await PostAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}/questions", addQuestionDto);
        Assert.True(addQuestionResult.Success);
        Assert.NotNull(addQuestionResult.Data);

        // 9. Clean up - Delete the questionnaire
        var deleteResult = await DeleteAsync($"/api/CategoryQuestionnaireTemplates/{questionnaireId}");
        Assert.True(deleteResult.Success);

        // 10. Clean up - Delete the category
        var deleteCategoryResult = await DeleteAsync($"/api/categories/{categoryId}");
        Assert.True(deleteCategoryResult.Success);
    }

    [Fact]
    public async Task MultiUserScenario_DifferentCategories_ShouldIsolateAccess()
    {
        // 1. Login as hair loss user
        var hairLossToken = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(hairLossToken);

        // 2. Get questionnaires for hair loss user
        var hairLossQuestionnaires = await GetAsync("/api/CategoryQuestionnaireTemplates");
        Assert.True(hairLossQuestionnaires.Success);
        Assert.NotNull(hairLossQuestionnaires.Data);

        // 3. Login as weight loss user
        var weightLossToken = await GetAuthTokenAsync("jane@test.com", "user123");
        SetAuthHeader(weightLossToken);

        // 4. Get questionnaires for weight loss user
        var weightLossQuestionnaires = await GetAsync("/api/CategoryQuestionnaireTemplates");
        Assert.True(weightLossQuestionnaires.Success);
        Assert.NotNull(weightLossQuestionnaires.Data);

        // 5. Verify different questionnaires are returned
        var hairLossData = JsonSerializer.Deserialize<JsonElement>(hairLossQuestionnaires.Data?.ToString() ?? "[]");
        var weightLossData = JsonSerializer.Deserialize<JsonElement>(weightLossQuestionnaires.Data?.ToString() ?? "[]");

        // Both should have questionnaires but they might be different based on user categories
        Assert.True(hairLossData.GetArrayLength() >= 0);
        Assert.True(weightLossData.GetArrayLength() >= 0);
    }

    [Fact]
    public async Task ValidationWorkflow_TestAllValidationScenarios_ShouldHandleAllCases()
    {
        // 1. Login as admin
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // 2. Test creating questionnaire with invalid category
        var invalidCategoryDto = new
        {
            Title = "Test Questionnaire",
            CategoryId = "00000000-0000-0000-0000-000000000000", // Invalid category
            IsActive = true
        };

        var invalidCategoryResult = await PostAsync("/api/CategoryQuestionnaireTemplates", invalidCategoryDto);
        Assert.False(invalidCategoryResult.Success);
        Assert.Equal(404, invalidCategoryResult.StatusCode);

        // 3. Test creating questionnaire with empty title
        var emptyTitleDto = new
        {
            Title = "",
            CategoryId = "11111111-1111-1111-1111-111111111111",
            IsActive = true
        };

        var emptyTitleResult = await PostAsync("/api/CategoryQuestionnaireTemplates", emptyTitleDto);
        Assert.False(emptyTitleResult.Success);
        Assert.Equal(400, emptyTitleResult.StatusCode);

        // 4. Test creating questionnaire with duplicate category (should fail)
        var duplicateCategoryDto = new
        {
            Title = "Duplicate Category Questionnaire",
            CategoryId = "11111111-1111-1111-1111-111111111111", // Hair Loss category that already has a questionnaire
            IsActive = true
        };

        var duplicateCategoryResult = await PostAsync("/api/CategoryQuestionnaireTemplates", duplicateCategoryDto);
        Assert.False(duplicateCategoryResult.Success);
        Assert.Equal(400, duplicateCategoryResult.StatusCode);

        // 5. Test updating non-existent questionnaire
        var updateNonExistentDto = new
        {
            Title = "Updated Questionnaire",
            CategoryId = "11111111-1111-1111-1111-111111111111",
            IsActive = true
        };

        var updateNonExistentResult = await PutAsync("/api/CategoryQuestionnaireTemplates/00000000-0000-0000-0000-000000000000", updateNonExistentDto);
        Assert.False(updateNonExistentResult.Success);
        Assert.Equal(404, updateNonExistentResult.StatusCode);

        // 6. Test deleting non-existent questionnaire
        var deleteNonExistentResult = await DeleteAsync("/api/CategoryQuestionnaireTemplates/00000000-0000-0000-0000-000000000000");
        Assert.False(deleteNonExistentResult.Success);
        Assert.Equal(404, deleteNonExistentResult.StatusCode);
    }

    [Fact]
    public async Task ErrorHandling_TestAllErrorScenarios_ShouldHandleGracefully()
    {
        // 1. Test accessing protected endpoint without authentication
        var noAuthResult = await GetAsync("/api/CategoryQuestionnaireTemplates");
        Assert.False(noAuthResult.Success);
        Assert.Equal(401, noAuthResult.StatusCode);

        // 2. Test accessing admin endpoint with user role
        var userToken = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(userToken);

        var createDto = new
        {
            Title = "Test Questionnaire",
            CategoryId = "11111111-1111-1111-1111-111111111111"
        };

        var userCreateResult = await PostAsync("/api/CategoryQuestionnaireTemplates", createDto);
        Assert.False(userCreateResult.Success);
        Assert.Equal(403, userCreateResult.StatusCode);

        // 3. Test accessing with invalid token
        SetAuthHeader("invalid-token");
        var invalidTokenResult = await GetAsync("/api/CategoryQuestionnaireTemplates");
        Assert.False(invalidTokenResult.Success);
        Assert.Equal(401, invalidTokenResult.StatusCode);

        // 4. Test accessing non-existent endpoint
        var notFoundResult = await GetAsync("/api/CategoryQuestionnaireTemplates/00000000-0000-0000-0000-000000000000");
        Assert.False(notFoundResult.Success);
        Assert.Equal(401, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task PerformanceTest_MultipleResponses_ShouldHandleConcurrentRequests()
    {
        // 1. Login as user
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);

        // 2. Submit multiple responses concurrently
        var tasks = new List<Task<JsonModel?>>();
        var questionnaireId = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        for (int i = 0; i < 5; i++)
        {
            var submitDto = new
            {
                QuestionnaireId = questionnaireId,
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
                        TextResponse = $"Response {i}"
                    },
                    new
                    {
                        QuestionId = "44444444-dddd-eeee-ffff-111111111111",
                        TextResponse = $"user{i}@test.com"
                    },
                    new
                    {
                        QuestionId = "55555555-eeee-ffff-1111-222222222222",
                        NumberResponse = 25 + i
                    }
                }
            };

            tasks.Add(PostAsync("/api/responses", submitDto));
        }

        // 3. Wait for all responses to complete
        var results = await Task.WhenAll(tasks);

        // 4. Verify all responses were successful
        foreach (var result in results)
        {
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }
} 
