using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Persistence;
using Nexora.Management.Tests.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.Tests.Infrastructure.Persistence;

public class AppDbContextTests : TestBase
{
    [Fact]
    public async Task SaveChangesAsync_SetsAuditableFields()
    {
        // Arrange
        var user = new User
        {
            Email = "audit@example.com",
            Name = "Audit User",
            PasswordHash = "hash"
        };

        DbContext.Users.Add(user);

        // Act
        await DbContext.SaveChangesAsync();

        // Assert
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChangesAsync_UpdatesUpdatedAt()
    {
        // Arrange
        var user = new User
        {
            Email = "update@example.com",
            Name = "Update User",
            PasswordHash = "hash"
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        var originalUpdatedAt = user.UpdatedAt;

        // Act - wait a bit and update
        await Task.Delay(10);
        user.Name = "Updated Name";
        await DbContext.SaveChangesAsync();

        // Assert
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task Users_CanAddAndRetrieve()
    {
        // Arrange
        var user = new User
        {
            Email = "retrieve@example.com",
            Name = "Retrieve User",
            PasswordHash = "hash"
        };

        // Act
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        var retrieved = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == "retrieve@example.com");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be("retrieve@example.com");
        retrieved.Name.Should().Be("Retrieve User");
    }

    [Fact]
    public async Task Workspaces_CanAddAndRetrieve()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            OwnerId = ownerId
        };

        // Act
        DbContext.Workspaces.Add(workspace);
        await DbContext.SaveChangesAsync();

        var retrieved = await DbContext.Workspaces.FirstOrDefaultAsync(w => w.OwnerId == ownerId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Workspace");
    }

    [Fact]
    public async Task DomainTasks_CanAddAndRetrieve()
    {
        // Arrange
        var taskListId = Guid.NewGuid();
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Test Task",
            TaskListId = taskListId,
            Priority = "high"
        };

        // Act
        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        var retrieved = await DbContext.Tasks.FirstOrDefaultAsync(t => t.TaskListId == taskListId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Title.Should().Be("Test Task");
        retrieved.Priority.Should().Be("high");
    }

    [Fact]
    public async Task SaveChangesAsync_GeneratesUniqueIds()
    {
        // Arrange
        var user1 = new User { Email = "user1@example.com", Name = "User 1", PasswordHash = "hash" };
        var user2 = new User { Email = "user2@example.com", Name = "User 2", PasswordHash = "hash" };

        // Act
        DbContext.Users.AddRange(user1, user2);
        await DbContext.SaveChangesAsync();

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        user1.Id.Should().NotBeEmpty();
        user2.Id.Should().NotBeEmpty();
    }
}
