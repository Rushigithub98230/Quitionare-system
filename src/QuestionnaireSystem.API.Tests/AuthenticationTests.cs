using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using QuestionnaireSystem.API.Tests;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace QuestionnaireSystem.API.Tests
{
    public class AuthenticationTests : TestBase
    {
        public AuthenticationTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Register_ValidUser_ShouldReturnToken()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            // Act
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<AuthResponse>(response);
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be("john.doe@test.com");
            result.User.Role.Should().Be("User");
        }

        [Fact]
        public async Task Register_AdminUser_ShouldReturnToken()
        {
            // Arrange
            var registerData = new
            {
                firstName = "Admin",
                lastName = "User",
                email = "admin@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "Admin"
            };

            // Act
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<AuthResponse>(response);
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.User.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task Register_DuplicateEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "duplicate@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            // Register first user
            await PostAsync("/api/auth/register", registerData);

            // Act - Try to register with same email
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_PasswordMismatch_ShouldReturnBadRequest()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@test.com",
                password = "password123",
                confirmPassword = "differentpassword",
                role = "User"
            };

            // Act
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_InvalidEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "invalid-email",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            // Act
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_ShortPassword_ShouldReturnBadRequest()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@test.com",
                password = "123",
                confirmPassword = "123",
                role = "User"
            };

            // Act
            var response = await PostAsync("/api/auth/register", registerData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            await PostAsync("/api/auth/register", registerData);

            var loginData = new
            {
                email = "john.doe@test.com",
                password = "password123"
            };

            // Act
            var response = await PostAsync("/api/auth/login", loginData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<AuthResponse>(response);
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be("john.doe@test.com");
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginData = new
            {
                email = "nonexistent@test.com",
                password = "wrongpassword"
            };

            // Act
            var response = await PostAsync("/api/auth/login", loginData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WrongPassword_ShouldReturnUnauthorized()
        {
            // Arrange
            var registerData = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@test.com",
                password = "password123",
                confirmPassword = "password123",
                role = "User"
            };

            await PostAsync("/api/auth/register", registerData);

            var loginData = new
            {
                email = "john.doe@test.com",
                password = "wrongpassword"
            };

            // Act
            var response = await PostAsync("/api/auth/login", loginData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ValidateToken_ValidToken_ShouldReturnUser()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<UserDto>(response);
            result.Should().NotBeNull();
            result!.Email.Should().Be("admin@test.com");
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task ValidateToken_InvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            SetAuthHeader("invalid-token");

            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ValidateToken_NoToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldAllowAccess()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AdminEndpoint_WithAdminToken_ShouldAllowAccess()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            SetAuthHeader(token);

            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AdminEndpoint_WithUserToken_ShouldAllowAccess()
        {
            // Arrange
            var token = await GetUserTokenAsync();
            SetAuthHeader(token);

            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ShouldReturnNewToken()
        {
            // Arrange
            var token = await GetAdminTokenAsync();
            var refreshData = new { refreshToken = "test-refresh-token" };

            // Act
            var response = await PostAsync("/api/auth/refresh", refreshData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeAsync<AuthResponse>(response);
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }
    }
} 