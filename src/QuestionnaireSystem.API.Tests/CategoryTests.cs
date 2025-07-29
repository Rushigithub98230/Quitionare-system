using FluentAssertions;
using QuestionnaireSystem.Core.Models;
using System.Net;

namespace QuestionnaireSystem.API.Tests
{
    public class CategoryTests : TestBase
    {
        public CategoryTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCategories_WithValidToken_ShouldReturnCategories()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await DeserializeAsync<List<CategoryDto>>(response);
            categories.Should().NotBeNull();
            categories!.Count.Should().BeGreaterThan(0);
            categories.Should().Contain(c => c.Id == category.Id.ToString());
        }

        [Fact]
        public async Task GetCategories_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetCategory_ValidId_ShouldReturnCategory()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            // Act
            var response = await _client.GetAsync($"/api/categories/{category.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<CategoryDto>(response);
            result.Should().NotBeNull();
            result!.Id.Should().Be(category.Id.ToString());
            result.Name.Should().Be(category.Name);
        }

        [Fact]
        public async Task GetCategory_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/categories/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCategory_ValidData_ShouldReturnCreatedCategory()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var categoryData = new
            {
                name = "New Test Category",
                description = "A new test category description",
                isActive = true
            };

            // Act
            var response = await PostAsync("/api/categories", categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await DeserializeAsync<CategoryDto>(response);
            result.Should().NotBeNull();
            result!.Name.Should().Be("New Test Category");
            result.Description.Should().Be("A new test category description");
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var categoryName = "Duplicate Category";
            await CreateTestCategoryAsync(categoryName);

            var categoryData = new
            {
                name = categoryName,
                description = "Another category with same name",
                isActive = true
            };

            // Act
            var response = await PostAsync("/api/categories", categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCategory_EmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var categoryData = new
            {
                name = "",
                description = "Category with empty name",
                isActive = true
            };

            // Act
            var response = await PostAsync("/api/categories", categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCategory_NullName_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var categoryData = new
            {
                name = (string?)null,
                description = "Category with null name",
                isActive = true
            };

            // Act
            var response = await PostAsync("/api/categories", categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCategory_WithoutToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var categoryData = new
            {
                name = "Unauthorized Category",
                description = "Category created without token",
                isActive = true
            };

            // Act
            var response = await PostAsync("/api/categories", categoryData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateCategory_ValidData_ShouldReturnUpdatedCategory()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var updateData = new
            {
                name = "Updated Category Name",
                description = "Updated description",
                isActive = false
            };

            // Act
            var response = await PutAsync($"/api/categories/{category.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<CategoryDto>(response);
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Category Name");
            result.Description.Should().Be("Updated description");
            result.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateCategory_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidId = Guid.NewGuid();
            var updateData = new
            {
                name = "Updated Category",
                description = "Updated description",
                isActive = true
            };

            // Act
            var response = await PutAsync($"/api/categories/{invalidId}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCategory_DuplicateName_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category1 = await CreateTestCategoryAsync("First Category");
            var category2 = await CreateTestCategoryAsync("Second Category");

            var updateData = new
            {
                name = "First Category", // Same name as category1
                description = "Updated description",
                isActive = true
            };

            // Act
            var response = await PutAsync($"/api/categories/{category2.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCategory_EmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var updateData = new
            {
                name = "",
                description = "Updated description",
                isActive = true
            };

            // Act
            var response = await PutAsync($"/api/categories/{category.Id}", updateData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteCategory_ValidId_ShouldReturnNoContent()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();

            // Act
            var response = await DeleteAsync($"/api/categories/{category.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify category is deleted
            var getResponse = await _client.GetAsync($"/api/categories/{category.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCategory_InvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var invalidId = Guid.NewGuid();

            // Act
            var response = await DeleteAsync($"/api/categories/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCategory_WithQuestionnaires_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var category = await CreateTestCategoryAsync();
            var questionnaire = await CreateTestQuestionnaireAsync(category);

            // Act
            var response = await DeleteAsync($"/api/categories/{category.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCategories_ActiveOnly_ShouldReturnActiveCategories()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            var activeCategory = await CreateTestCategoryAsync("Active Category");
            var inactiveCategory = await CreateTestCategoryAsync("Inactive Category");
            
            // Deactivate one category
            inactiveCategory.IsActive = false;
            _context.Categories.Update(inactiveCategory);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/categories?activeOnly=true");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await DeserializeAsync<List<CategoryDto>>(response);
            categories.Should().NotBeNull();
            categories!.Should().OnlyContain(c => c.IsActive);
        }

        [Fact]
        public async Task GetCategories_WithPagination_ShouldReturnPaginatedResults()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            // Create multiple categories
            for (int i = 1; i <= 5; i++)
            {
                await CreateTestCategoryAsync($"Category {i}");
            }

            // Act
            var response = await _client.GetAsync("/api/categories?page=1&pageSize=3");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await DeserializeAsync<List<CategoryDto>>(response);
            categories.Should().NotBeNull();
            categories!.Count.Should().BeLessThanOrEqualTo(3);
        }

        [Fact]
        public async Task GetCategories_WithSearch_ShouldReturnFilteredResults()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            await CreateTestCategoryAsync("Health Category");
            await CreateTestCategoryAsync("Education Category");
            await CreateTestCategoryAsync("Technology Category");

            // Act
            var response = await _client.GetAsync("/api/categories?search=Health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await DeserializeAsync<List<CategoryDto>>(response);
            categories.Should().NotBeNull();
            categories!.Should().OnlyContain(c => c.Name.Contains("Health"));
        }

        protected class CategoryDto
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public string CreatedAt { get; set; } = string.Empty;
            public string? UpdatedAt { get; set; }
        }
    }
} 