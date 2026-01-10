using FluentAssertions;
using Nexora.Management.Domain.Entities;
using Xunit;

namespace Nexora.Management.Tests.Core.Entities;

public class WorkspaceEntityTests
{
    [Fact]
    public void CreateWorkspace_WithValidData_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            OwnerId = ownerId
        };

        // Assert
        workspace.Name.Should().Be("Test Workspace");
        workspace.OwnerId.Should().Be(ownerId);
        // ID is not generated until saved to database
        workspace.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CreateWorkspace_WithSettings_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            OwnerId = ownerId,
            SettingsJsonb = new Dictionary<string, object>
            {
                { "theme", "dark" },
                { "language", "en" }
            }
        };

        // Assert
        workspace.SettingsJsonb.Should().ContainKey("theme");
        workspace.SettingsJsonb["theme"].Should().Be("dark");
    }

    [Fact]
    public void CreateWorkspace_DefaultsToEmptySettings()
    {
        // Arrange & Act
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            OwnerId = Guid.NewGuid()
        };

        // Assert
        workspace.SettingsJsonb.Should().NotBeNull();
        workspace.SettingsJsonb.Should().BeEmpty();
    }
}
