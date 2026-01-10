using FluentAssertions;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Tests.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.Tests.Application.Tasks;

public class TaskEntityTests : TestBase
{
    private readonly TestDataBuilder _dataBuilder;

    public TaskEntityTests()
    {
        _dataBuilder = new TestDataBuilder(DbContext);
    }

    [Fact]
    public async Task TaskEntity_CanBeCreated()
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

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("Test Task");
        task.Description.Should().Be("Test Description");
        task.TaskListId.Should().Be(taskListId);
        task.AssigneeId.Should().Be(assigneeId);
        task.Priority.Should().Be("high");
    }

    [Fact]
    public async Task TaskEntity_WithParentTask_Succeeds()
    {
        // Arrange
        var taskListId = Guid.NewGuid();
        var parentTaskId = Guid.NewGuid();

        // Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Child Task",
            TaskListId = taskListId,
            ParentTaskId = parentTaskId
        };

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.ParentTaskId.Should().Be(parentTaskId);
        task.Title.Should().Be("Child Task");
    }

    [Fact]
    public async Task TaskEntity_WithDueDate_Succeeds()
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

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task TaskEntity_WithEstimatedHours_Succeeds()
    {
        // Arrange & Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Estimate",
            TaskListId = Guid.NewGuid(),
            EstimatedHours = 8.5m
        };

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.EstimatedHours.Should().Be(8.5m);
    }

    [Fact]
    public async Task TaskEntity_WithCustomFields_Succeeds()
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

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.CustomFieldsJsonb.Should().ContainKey("sprint");
        task.CustomFieldsJsonb["sprint"].Should().Be("Sprint 1");
    }

    [Fact]
    public async Task TaskEntity_WithPositionOrder_Succeeds()
    {
        // Arrange & Act
        var task = new Nexora.Management.Domain.Entities.Task
        {
            Title = "Task with Position",
            TaskListId = Guid.NewGuid(),
            PositionOrder = 5
        };

        DbContext.Tasks.Add(task);
        await DbContext.SaveChangesAsync();

        // Assert
        task.PositionOrder.Should().Be(5);
    }
}
