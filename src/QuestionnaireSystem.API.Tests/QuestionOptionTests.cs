using FluentAssertions;
using QuestionnaireSystem.Core.Models;
using System.Net;

namespace QuestionnaireSystem.API.Tests
{
    public class QuestionOptionTests : TestBase
    {
        public QuestionOptionTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateQuestionOption_ValidData_ShouldReturnCreatedOption()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6); // Radio type

            var optionData = new
            {
                text = "New Option",
                order = 1,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionOptionDto>(response);
            result.Should().NotBeNull();
            result!.Text.Should().Be("New Option");
            result.Order.Should().Be(1);
            result.QuestionId.Should().Be(question.Id.ToString());
        }

        [Fact]
        public async Task CreateQuestionOption_EmptyText_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var optionData = new
            {
                text = "",
                order = 1,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateQuestionOption_NullText_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var optionData = new
            {
                text = (string?)null,
                order = 1,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateQuestionOption_InvalidQuestionId_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidQuestionId = Guid.NewGuid();

            var optionData = new
            {
                text = "New Option",
                order = 1,
                questionId = invalidQuestionId.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateQuestionOption_ForNonOptionQuestionType_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1); // Text type (no options)

            var optionData = new
            {
                text = "New Option",
                order = 1,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateQuestionOption_DuplicateText_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            // Create first option
            var option1 = await CreateTestOptionAsync(question);

            var optionData = new
            {
                text = option1.Text, // Same text
                order = 2,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PostAsync("/api/question-options", optionData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetQuestionOptions_ValidQuestionId_ShouldReturnOptions()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var option1 = await CreateTestOptionAsync(question);
            var option2 = await CreateTestOptionAsync(question);

            // Act
            var response = await _client.GetAsync($"/api/question-options/question/{question.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var options = await DeserializeAsync<List<QuestionOptionDto>>(response);
            options.Should().NotBeNull();
            options!.Count.Should().Be(2);
            options.Should().Contain(o => o.Id == option1.Id.ToString());
            options.Should().Contain(o => o.Id == option2.Id.ToString());
        }

        [Fact]
        public async Task GetQuestionOptions_InvalidQuestionId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidQuestionId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/question-options/question/{invalidQuestionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetQuestionOption_ValidId_ShouldReturnOption()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);
            var option = await CreateTestOptionAsync(question);

            // Act
            var response = await _client.GetAsync($"/api/question-options/{option.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<QuestionOptionDto>(response);
            result.Should().NotBeNull();
            result!.Id.Should().Be(option.Id.ToString());
            result.Text.Should().Be(option.Text);
        }

        [Fact]
        public async Task GetQuestionOption_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/question-options/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateQuestionOption_ValidData_ShouldReturnUpdatedOption()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);
            var option = await CreateTestOptionAsync(question);

            var updateData = new
            {
                text = "Updated Option Text",
                order = 5,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PutAsync($"/api/question-options/{option.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<QuestionOptionDto>(response);
            result.Should().NotBeNull();
            result!.Text.Should().Be("Updated Option Text");
            result.Order.Should().Be(5);
        }

        [Fact]
        public async Task UpdateQuestionOption_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var invalidId = Guid.NewGuid();
            var updateData = new
            {
                text = "Updated Option Text",
                order = 5,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PutAsync($"/api/question-options/{invalidId}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateQuestionOption_EmptyText_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);
            var option = await CreateTestOptionAsync(question);

            var updateData = new
            {
                text = "",
                order = 5,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PutAsync($"/api/question-options/{option.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateQuestionOption_DuplicateText_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var option1 = await CreateTestOptionAsync(question);
            var option2 = await CreateTestOptionAsync(question);

            var updateData = new
            {
                text = option1.Text, // Same text as option1
                order = 5,
                questionId = question.Id.ToString()
            };

            // Act
            var response = await PutAsync($"/api/question-options/{option2.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteQuestionOption_ValidId_ShouldReturnNoContent()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);
            var option = await CreateTestOptionAsync(question);

            // Act
            var response = await DeleteAsync($"/api/question-options/{option.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify option is deleted
            var getResponse = await _client.GetAsync($"/api/question-options/{option.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteQuestionOption_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidId = Guid.NewGuid();

            // Act
            var response = await DeleteAsync($"/api/question-options/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ReorderQuestionOptions_ValidData_ShouldReturnUpdatedOrder()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6);

            var option1 = await CreateTestOptionAsync(question);
            var option2 = await CreateTestOptionAsync(question);

            var reorderData = new
            {
                optionIds = new[] { option2.Id.ToString(), option1.Id.ToString() }
            };

            // Act
            var response = await PutAsync($"/api/question-options/reorder/{question.Id}", reorderData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify new order
            var getResponse = await _client.GetAsync($"/api/question-options/question/{question.Id}");
            var options = await DeserializeAsync<List<QuestionOptionDto>>(getResponse);
            options.Should().NotBeNull();
            options![0].Id.Should().Be(option2.Id.ToString());
            options[1].Id.Should().Be(option1.Id.ToString());
        }

        [Fact]
        public async Task CreateMultipleOptions_ForRadioQuestion_ShouldReturnAllOptions()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 6); // Radio

            var optionsData = new[]
            {
                new { text = "Option 1", order = 1 },
                new { text = "Option 2", order = 2 },
                new { text = "Option 3", order = 3 },
                new { text = "Option 4", order = 4 }
            };

            // Act
            foreach (var optionData in optionsData)
            {
                var fullOptionData = new
                {
                    text = optionData.text,
                    order = optionData.order,
                    questionId = question.Id.ToString()
                };

                var response = await PostAsync("/api/question-options", fullOptionData);
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            // Assert
            var getResponse = await _client.GetAsync($"/api/question-options/question/{question.Id}");
            var options = await DeserializeAsync<List<QuestionOptionDto>>(getResponse);
            options.Should().NotBeNull();
            options!.Count.Should().Be(4);
            options.Should().BeInAscendingOrder(o => o.Order);
        }

        [Fact]
        public async Task CreateMultipleOptions_ForCheckboxQuestion_ShouldReturnAllOptions()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 7); // Checkbox

            var optionsData = new[]
            {
                new { text = "Choice 1", order = 1 },
                new { text = "Choice 2", order = 2 },
                new { text = "Choice 3", order = 3 },
                new { text = "Choice 4", order = 4 },
                new { text = "Choice 5", order = 5 }
            };

            // Act
            foreach (var optionData in optionsData)
            {
                var fullOptionData = new
                {
                    text = optionData.text,
                    order = optionData.order,
                    questionId = question.Id.ToString()
                };

                var response = await PostAsync("/api/question-options", fullOptionData);
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            // Assert
            var getResponse = await _client.GetAsync($"/api/question-options/question/{question.Id}");
            var options = await DeserializeAsync<List<QuestionOptionDto>>(getResponse);
            options.Should().NotBeNull();
            options!.Count.Should().Be(5);
            options.Should().BeInAscendingOrder(o => o.Order);
        }

        [Fact]
        public async Task CreateMultipleOptions_ForDropdownQuestion_ShouldReturnAllOptions()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 8); // Dropdown

            var optionsData = new[]
            {
                new { text = "Select Option 1", order = 1 },
                new { text = "Select Option 2", order = 2 },
                new { text = "Select Option 3", order = 3 }
            };

            // Act
            foreach (var optionData in optionsData)
            {
                var fullOptionData = new
                {
                    text = optionData.text,
                    order = optionData.order,
                    questionId = question.Id.ToString()
                };

                var response = await PostAsync("/api/question-options", fullOptionData);
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            // Assert
            var getResponse = await _client.GetAsync($"/api/question-options/question/{question.Id}");
            var options = await DeserializeAsync<List<QuestionOptionDto>>(getResponse);
            options.Should().NotBeNull();
            options!.Count.Should().Be(3);
            options.Should().BeInAscendingOrder(o => o.Order);
        }

        protected class QuestionOptionDto
        {
            public string Id { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public int Order { get; set; }
            public string QuestionId { get; set; } = string.Empty;
            public string CreatedAt { get; set; } = string.Empty;
            public string? UpdatedAt { get; set; }
        }
    }
} 