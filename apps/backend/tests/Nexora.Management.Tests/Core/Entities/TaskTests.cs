using FluentAssertions;
using Nexora.Management.Domain.Entities;
using Xunit;

namespace Nexora.Management.Tests.Core.Entities;

public class TaskEntityTests
{
    [Fact]
    public void CreateTask_WithValidData_Succeeds()
    {
        // Arrange
        var taskListId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();

        // Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Test Task",
            Description = "Test Description",
            TaskListId = taskListId,
            AssigneeId = assigneeId,
            Priority = "high"
        };

        // Assert
        task.Title.Should().Be("Test Task");
        task.Description.Should().Be("Test Description");
        task.TaskListId.Should().Be(taskListId);
        task.AssigneeId.Should().Be(assigneeId);
        task.Priority.Should().Be("high");
        // ID is not generated until saved to database
        task.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CreateTask_WithParentTaskId_Succeeds()
    {
        // Arrange
        var parentTaskId = Guid.NewGuid();
        var taskListId = Guid.NewGuid();

        // Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Child Task",
            TaskListId = taskListId,
            ParentTaskId = parentTaskId
        };

        // Assert
        task.ParentTaskId.Should().Be(parentTaskId);
        task.Title.Should().Be("Child Task");
    }

    [Fact]
    public void CreateTask_WithDueDate_Succeeds()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(7);

        // Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Due Date",
            TaskListId = Guid.NewGuid(),
            DueDate = dueDate
        };

        // Assert
        task.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateTask_WithEstimatedHours_Succeeds()
    {
        // Arrange & Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Estimate",
            TaskListId = Guid.NewGuid(),
            EstimatedHours = 8.5m
        };

        // Assert
        task.EstimatedHours.Should().Be(8.5m);
    }

    [Fact]
    public void CreateTask_WithCustomFields_Succeeds()
    {
        // Arrange & Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Custom Fields",
            TaskListId = Guid.NewGuid(),
            CustomFieldsJsonb = new Dictionary<string, object>
            {
                { "sprint", "Sprint 1" },
                { "story_points", 5 }
            }
        };

        // Assert
        task.CustomFieldsJsonb.Should().ContainKey("sprint");
        task.CustomFieldsJsonb["sprint"].Should().Be("Sprint 1");
    }

    [Fact]
    public void CreateTask_WithPositionOrder_Succeeds()
    {
        // Arrange & Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Position",
            TaskListId = Guid.NewGuid(),
            PositionOrder = 5
        };

        // Assert
        task.PositionOrder.Should().Be(5);
    }
}
