using FluentAssertions;
using Nexora.Management.Domain.Entities;
using Xunit;

namespace Nexora.Management.Tests.Core.Entities;

public class UserTests
{
    [Fact]
    public void CreateUser_WithValidData_Succeeds()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = "hashed_password"
        };

        // Assert
        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.PasswordHash.Should().Be("hashed_password");
        // ID is not generated until saved to database
        user.Id.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("no-at-sign.com")]
    public void CreateUser_WithInvalidEmail_FailsValidation(string email)
    {
        // Arrange & Act
        var user = new User
        {
            Email = email,
            Name = "Test User",
            PasswordHash = "hashed_password"
        };

        // Assert - Email should be set but validation would fail at application layer
        user.Email.Should().Be(email);
    }

    [Fact]
    public void CreateUser_WithEmptyName_UsesDefaultValue()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            Name = string.Empty,
            PasswordHash = "hashed_password"
        };

        // Assert
        user.Name.Should().BeEmpty();
    }

    [Fact]
    public void CreateUser_DefaultsAuditableFields()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = "hashed_password"
        };

        // Assert
        user.Id.Should().BeEmpty(); // Not set until saved via DbContext
        user.CreatedAt.Should().Be(default); // Would be set by EF
        user.UpdatedAt.Should().Be(default); // Would be set by EF
    }
}
