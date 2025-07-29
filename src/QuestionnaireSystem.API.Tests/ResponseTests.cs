using FluentAssertions;
using QuestionnaireSystem.Core.Models;
using System.Net;

namespace QuestionnaireSystem.API.Tests
{
    public class ResponseTests : TestBase
    {
        public ResponseTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task SubmitResponse_ValidData_ShouldReturnCreatedResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1); // Text type

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "This is my answer"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<ResponseDto>(response);
            result.Should().NotBeNull();
            result!.QuestionnaireId.Should().Be(questionnaire.Id.ToString());
        }

        [Fact]
        public async Task SubmitResponse_WithTextQuestion_ShouldSaveTextResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1); // Text

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "Sample text response"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithNumberQuestion_ShouldSaveNumberResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 2); // Number

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        numberResponse = 25
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithEmailQuestion_ShouldSaveEmailResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 3); // Email

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "test@example.com"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithPhoneQuestion_ShouldSavePhoneResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 4); // Phone

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "+1234567890"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithDateQuestion_ShouldSaveDateResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 5); // Date

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        dateResponse = "2024-01-15"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithRadioQuestion_ShouldSaveOptionResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6); // Radio
            var option = await CreateTestOptionAsync(question);

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        optionResponses = new[]
                        {
                            new { optionId = option.Id.ToString() }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithCheckboxQuestion_ShouldSaveMultipleOptionResponses()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 7); // Checkbox
            var option1 = await CreateTestOptionAsync(question);
            var option2 = await CreateTestOptionAsync(question);

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        optionResponses = new[]
                        {
                            new { optionId = option1.Id.ToString() },
                            new { optionId = option2.Id.ToString() }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithDropdownQuestion_ShouldSaveOptionResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionnaireAsync(questionnaire, 8); // Dropdown
            var option = await CreateTestOptionAsync(question);

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        optionResponses = new[]
                        {
                            new { optionId = option.Id.ToString() }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithRatingQuestion_ShouldSaveRatingResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 9); // Rating

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        numberResponse = 4
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithSliderQuestion_ShouldSaveSliderResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 10); // Slider

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        numberResponse = 75
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithYesNoQuestion_ShouldSaveBooleanResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 11); // Yes/No

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        booleanResponse = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_WithTextAreaQuestion_ShouldSaveLongTextResponse()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 14); // Text Area

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "This is a detailed response with multiple sentences. It contains more information than a simple text input would typically allow."
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task SubmitResponse_MissingRequiredQuestion_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1); // Required text question

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new object[] { } // No responses
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_InvalidQuestionnaireId_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var invalidQuestionnaireId = Guid.NewGuid();

            var responseData = new
            {
                questionnaireId = invalidQuestionnaireId.ToString(),
                responses = new object[] { }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_InvalidQuestionId_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var invalidQuestionId = Guid.NewGuid();

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = invalidQuestionId.ToString(),
                        textResponse = "Some response"
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_TextTooShort_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1);
            
            // Update question to have min length validation
            question.MinLength = 10;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "Short" // Less than 10 characters
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_TextTooLong_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1);
            
            // Update question to have max length validation
            question.MaxLength = 5;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "This is too long" // More than 5 characters
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_NumberTooSmall_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 2); // Number
            
            // Update question to have min value validation
            question.MinValue = 18;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        numberResponse = 15 // Less than 18
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_NumberTooLarge_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 2); // Number
            
            // Update question to have max value validation
            question.MaxValue = 100;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        numberResponse = 150 // More than 100
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitResponse_InvalidEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 3); // Email

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "invalid-email" // Invalid email format
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/responses", responseData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetResponses_ByQuestionnaire_ShouldReturnResponses()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1);

            // Submit a response first
            var userToken = await GetUserTokenAsync();
            SetAuthHeader(userToken);

            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "Test response"
                    }
                }
            };

            await PostAsync("/api/responses", responseData);

            // Switch back to admin token
            SetAuthHeader(token);

            // Act
            var response = await _client.GetAsync($"/api/responses/questionnaire/{questionnaire.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var results = await DeserializeAsync<List<ResponseSummaryDto>>(response);
            results.Should().NotBeNull();
            results!.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetResponses_ByUser_ShouldReturnUserResponses()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1);

            // Submit a response
            var responseData = new
            {
                questionnaireId = questionnaire.Id.ToString(),
                responses = new[]
                {
                    new
                    {
                        questionId = question.Id.ToString(),
                        textResponse = "Test response"
                    }
                }
            };

            await PostAsync("/api/responses", responseData);

            // Act
            var response = await _client.GetAsync("/api/responses/my-responses");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var results = await DeserializeAsync<List<ResponseSummaryDto>>(response);
            results.Should().NotBeNull();
            results!.Count.Should().BeGreaterThan(0);
        }

        protected class ResponseDto
        {
            public string Id { get; set; } = string.Empty;
            public string QuestionnaireId { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string CreatedAt { get; set; } = string.Empty;
        }

        protected class ResponseSummaryDto
        {
            public string Id { get; set; } = string.Empty;
            public string QuestionnaireTitle { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public string CreatedAt { get; set; } = string.Empty;
        }
    }
} 