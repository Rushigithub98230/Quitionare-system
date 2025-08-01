using Xunit;

namespace QuestionnaireSystem.API.Tests;

public class AuthenticationTests : TestBase
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new { Email = "admin@test.com", Password = "admin123" };

        // Act
        var result = await PostAsync("/api/auth/login", loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Login successful", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnError()
    {
        // Arrange
        var loginDto = new { Email = "admin@test.com", Password = "wrongpassword" };

        // Act
        var result = await PostAsync("/api/auth/login", loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
        Assert.Contains("Invalid email or password", result.Message);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldReturnError()
    {
        // Arrange
        var loginDto = new { Email = "nonexistent@test.com", Password = "password" };

        // Act
        var result = await PostAsync("/api/auth/login", loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
        Assert.Contains("Invalid email or password", result.Message);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var registerDto = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = "newuser@test.com",
            Password = "password123",
            confirmPassword = "password123",
            Role = "User",
            Category = "Hair Loss"
        };

        // Act
        var result = await PostAsync("/api/auth/register", registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("Registration successful", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var registerDto = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = "admin@test.com", // Already exists
            Password = "password123",
            confirmPassword = "password123",
            Role = "User",
            Category = "Hair Loss"
        };

        // Act
        var result = await PostAsync("/api/auth/register", registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Contains("User with this email already exists", result.Message);
    }

    [Fact]
    public async Task ValidateToken_WithValidToken_ShouldReturnSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // Act
        var result = await PostAsync("/api/auth/validate", new { });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ValidateToken_WithoutToken_ShouldReturnError()
    {
        // Act
        var result = await PostAsync("/api/auth/validate", new { });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
        Assert.Contains("Unauthorized", result.Message);
    }

    [Fact]
    public async Task GetUserProfile_WithValidToken_ShouldReturnUserData()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@test.com", "admin123");
        SetAuthHeader(token);

        // Act
        var result = await GetAsync("/api/auth/profile");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("User profile retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetUserProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var result = await GetAsync("/api/auth/profile");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }
} 