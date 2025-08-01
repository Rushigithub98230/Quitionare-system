using Xunit;

namespace QuestionnaireSystem.API.Tests;

public class CategoryTests : TestBase
{
    [Fact]
    public async Task GetAllCategories_WithValidToken_ShouldReturnCategories()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/categories");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Categories retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAllCategories_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var result = await GetAsync("/api/categories");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetCategoryById_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "11111111-1111-1111-1111-111111111111";

        // Act
        var result = await GetAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Category retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetCategoryById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "00000000-0000-0000-0000-000000000000";

        // Act
        var result = await GetAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Category not found", result.Message);
    }

    [Fact]
    public async Task CreateCategory_WithAdminToken_ShouldCreateCategory()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var createDto = new
        {
            Name = "Test Category",
            Description = "Test category description",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 10
        };

        // Act
        var result = await PostAsync("/api/categories", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Category created successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateCategory_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var createDto = new
        {
            Name = "Test Category",
            Description = "Test category description",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 10
        };

        // Act
        var result = await PostAsync("/api/categories", createDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task UpdateCategory_WithAdminToken_ShouldUpdateCategory()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "11111111-1111-1111-1111-111111111111";
        var updateDto = new
        {
            Name = "Updated Hair Loss",
            Description = "Updated description",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 5
        };

        // Act
        var result = await PutAsync($"/api/categories/{categoryId}", updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Category updated successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "00000000-0000-0000-0000-000000000000";
        var updateDto = new
        {
            Name = "Updated Category",
            Description = "Updated description",
            Color = "#FF0000",
            IsActive = true,
            DisplayOrder = 5
        };

        // Act
        var result = await PutAsync($"/api/categories/{categoryId}", updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Category not found", result.Message);
    }

    [Fact]
    public async Task DeleteCategory_WithAdminToken_ShouldDeleteCategory()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "22222222-2222-2222-2222-222222222222";

        // Act
        var result = await DeleteAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Category deleted successfully", result.Message);
    }

    [Fact]
    public async Task DeleteCategory_WithUserToken_ShouldReturnForbidden()
    {
        // Arrange
        var token = await GetAuthTokenAsync("john@test.com", "user123");
        SetAuthHeader(token);
        var categoryId = "11111111-1111-1111-1111-111111111111";

        // Act
        var result = await DeleteAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Contains("Access denied", result.Message);
    }

    [Fact]
    public async Task DeleteCategory_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);
        var categoryId = "00000000-0000-0000-0000-000000000000";

        // Act
        var result = await DeleteAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Category not found", result.Message);
    }
} 