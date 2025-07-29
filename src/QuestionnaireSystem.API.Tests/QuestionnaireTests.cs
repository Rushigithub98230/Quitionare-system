using FluentAssertions;
using QuestionnaireSystem.Core.Models;
using System.Net;

namespace QuestionnaireSystem.API.Tests
{
    public class QuestionnaireTests : TestBase
    {
        public QuestionnaireTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetQuestionnaires_WithValidToken_ShouldReturnQuestionnaires()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);

            // Act
            var response = await _client.GetAsync("/api/questionnaires");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var questionnaires = await DeserializeAsync<List<QuestionnaireDto>>(response);
            questionnaires.Should().NotBeNull();
            questionnaires!.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetQuestionnaire_ValidId_ShouldReturnQuestionnaireWithQuestions()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);
            var question = await CreateTestQuestionAsync(questionnaire, 1); // Text type
            var option = await CreateTestOptionAsync(question);

            // Act
            var response = await _client.GetAsync($"/api/questionnaires/{questionnaire.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Id.Should().Be(questionnaire.Id.ToString());
            result.Questions.Should().NotBeNull();
            result.Questions!.Count.Should().Be(1);
            result.Questions[0].Options.Should().NotBeNull();
            result.Questions[0].Options!.Count.Should().Be(1);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithTextQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Text Question",
                description = "A questionnaire with text input question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your name?",
                        questionTypeId = 1, // Text
                        order = 1,
                        isRequired = true,
                        minLength = 2,
                        maxLength = 50
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Test Questionnaire with Text Question");
            result.Questions.Should().NotBeNull();
            result.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(1);
            result.Questions[0].MinLength.Should().Be(2);
            result.Questions[0].MaxLength.Should().Be(50);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithNumberQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Number Question",
                description = "A questionnaire with number input question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your age?",
                        questionTypeId = 2, // Number
                        order = 1,
                        isRequired = true,
                        minValue = 18,
                        maxValue = 100
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(2);
            result.Questions[0].MinValue.Should().Be(18);
            result.Questions[0].MaxValue.Should().Be(100);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithEmailQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Email Question",
                description = "A questionnaire with email input question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your email address?",
                        questionTypeId = 3, // Email
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(3);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithPhoneQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Phone Question",
                description = "A questionnaire with phone input question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your phone number?",
                        questionTypeId = 4, // Phone
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(4);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithDateQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Date Question",
                description = "A questionnaire with date picker question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your birth date?",
                        questionTypeId = 5, // Date
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(5);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithRadioQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Radio Question",
                description = "A questionnaire with single choice question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your gender?",
                        questionTypeId = 6, // Radio
                        order = 1,
                        isRequired = true,
                        options = new[]
                        {
                            new { text = "Male", order = 1 },
                            new { text = "Female", order = 2 },
                            new { text = "Other", order = 3 }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(6);
            result.Questions[0].Options.Should().NotBeNull();
            result.Questions[0].Options!.Count.Should().Be(3);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithCheckboxQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Checkbox Question",
                description = "A questionnaire with multiple choice question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What programming languages do you know?",
                        questionTypeId = 7, // Checkbox
                        order = 1,
                        isRequired = true,
                        options = new[]
                        {
                            new { text = "C#", order = 1 },
                            new { text = "JavaScript", order = 2 },
                            new { text = "Python", order = 3 },
                            new { text = "Java", order = 4 }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(7);
            result.Questions[0].Options.Should().NotBeNull();
            result.Questions[0].Options!.Count.Should().Be(4);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithDropdownQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Dropdown Question",
                description = "A questionnaire with dropdown question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your education level?",
                        questionTypeId = 8, // Dropdown
                        order = 1,
                        isRequired = true,
                        options = new[]
                        {
                            new { text = "High School", order = 1 },
                            new { text = "Bachelor's Degree", order = 2 },
                            new { text = "Master's Degree", order = 3 },
                            new { text = "PhD", order = 4 }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(8);
            result.Questions[0].Options.Should().NotBeNull();
            result.Questions[0].Options!.Count.Should().Be(4);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithRatingQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Rating Question",
                description = "A questionnaire with rating question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "Rate our service from 1 to 5",
                        questionTypeId = 9, // Rating
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(9);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithSliderQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Slider Question",
                description = "A questionnaire with slider question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "How satisfied are you? (0-100)",
                        questionTypeId = 10, // Slider
                        order = 1,
                        isRequired = true,
                        minValue = 0,
                        maxValue = 100
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(10);
            result.Questions[0].MinValue.Should().Be(0);
            result.Questions[0].MaxValue.Should().Be(100);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithYesNoQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Yes/No Question",
                description = "A questionnaire with yes/no question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "Do you agree to the terms and conditions?",
                        questionTypeId = 11, // Yes/No
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(11);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithFileUploadQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with File Upload Question",
                description = "A questionnaire with file upload question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "Please upload your resume",
                        questionTypeId = 12, // File Upload
                        order = 1,
                        isRequired = true
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(12);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithImageQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Image Question",
                description = "A questionnaire with image display question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "Please look at the image below",
                        questionTypeId = 13, // Image
                        order = 1,
                        isRequired = false
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(13);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithTextAreaQuestion_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Text Area Question",
                description = "A questionnaire with long text question",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "Please describe your experience in detail",
                        questionTypeId = 14, // Text Area
                        order = 1,
                        isRequired = true,
                        minLength = 10,
                        maxLength = 1000
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(1);
            result.Questions[0].QuestionTypeId.Should().Be(14);
            result.Questions[0].MinLength.Should().Be(10);
            result.Questions[0].MaxLength.Should().Be(1000);
        }

        [Fact]
        public async Task CreateQuestionnaire_WithMultipleQuestions_ShouldReturnCreatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            var questionnaireData = new
            {
                title = "Test Questionnaire with Multiple Questions",
                description = "A questionnaire with various question types",
                categoryId = category.Id.ToString(),
                version = "1.0",
                isActive = true,
                questions = new[]
                {
                    new
                    {
                        text = "What is your name?",
                        questionTypeId = 1, // Text
                        order = 1,
                        isRequired = true
                    },
                    new
                    {
                        text = "What is your age?",
                        questionTypeId = 2, // Number
                        order = 2,
                        isRequired = true
                    },
                    new
                    {
                        text = "What is your gender?",
                        questionTypeId = 6, // Radio
                        order = 3,
                        isRequired = true,
                        options = new[]
                        {
                            new { text = "Male", order = 1 },
                            new { text = "Female", order = 2 }
                        }
                    }
                }
            };

            // Act
            var response = await PostAsync("/api/questionnaires", questionnaireData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Questions!.Count.Should().Be(3);
            result.Questions[0].QuestionTypeId.Should().Be(1);
            result.Questions[1].QuestionTypeId.Should().Be(2);
            result.Questions[2].QuestionTypeId.Should().Be(6);
        }

        [Fact]
        public async Task UpdateQuestionnaire_ValidData_ShouldReturnUpdatedQuestionnaire()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);

            var updateData = new
            {
                title = "Updated Questionnaire Title",
                description = "Updated description",
                categoryId = category.Id.ToString(),
                version = "2.0",
                isActive = false,
                questions = new[]
                {
                    new
                    {
                        text = "Updated question text",
                        questionTypeId = 1,
                        order = 1,
                        isRequired = false
                    }
                }
            };

            // Act
            var response = await PutAsync($"/api/questionnaires/{questionnaire.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<QuestionnaireDetailDto>(response);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Questionnaire Title");
            result.Version.Should().Be("2.0");
            result.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteQuestionnaire_ValidId_ShouldReturnNoContent()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);

            // Act
            var response = await DeleteAsync($"/api/questionnaires/{questionnaire.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify questionnaire is deleted
            var getResponse = await _client.GetAsync($"/api/questionnaires/{questionnaire.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetQuestionnaires_ByCategory_ShouldReturnFilteredResults()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category1 = await CreateTestCategoryAsync("Category 1");
            var category2 = await CreateTestCategoryAsync("Category 2");

            await CreateTestQuestionnaireAsync(category1);
            await CreateTestQuestionnaireAsync(category1);
            await CreateTestQuestionnaireAsync(category2);

            // Act
            var response = await _client.GetAsync($"/api/questionnaires?categoryId={category1.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var questionnaires = await DeserializeAsync<List<QuestionnaireDto>>(response);
            questionnaires.Should().NotBeNull();
            questionnaires!.Should().OnlyContain(q => q.CategoryId == category1.Id.ToString());
        }

        protected class QuestionnaireDto
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CategoryId { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public string CreatedAt { get; set; } = string.Empty;
            public string? UpdatedAt { get; set; }
        }

        protected class QuestionnaireDetailDto : QuestionnaireDto
        {
            public List<QuestionDto>? Questions { get; set; }
        }

        protected class QuestionDto
        {
            public string Id { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public int QuestionTypeId { get; set; }
            public int Order { get; set; }
            public bool IsRequired { get; set; }
            public int? MinLength { get; set; }
            public int? MaxLength { get; set; }
            public int? MinValue { get; set; }
            public int? MaxValue { get; set; }
            public List<QuestionOptionDto>? Options { get; set; }
        }

        protected class QuestionOptionDto
        {
            public string Id { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public int Order { get; set; }
        }
    }
} 