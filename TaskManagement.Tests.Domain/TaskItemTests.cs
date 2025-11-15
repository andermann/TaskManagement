using System;
using FluentAssertions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.Tests.Domain
{
    public class TaskItemTests
    {
        [Fact]
        public void Constructor_ShouldInitializeFieldsCorrectly()
        {
            var dueDate = DateTime.UtcNow.AddDays(5);
            var task = new TaskItem(1, "Título", "Descrição", dueDate, TaskPriority.Medium);

            task.ProjectId.Should().Be(1);
            task.Title.Should().Be("Título");
            task.Description.Should().Be("Descrição");
            task.DueDate.Should().Be(dueDate);
            task.Priority.Should().Be(TaskPriority.Medium);
            task.Status.Should().Be(TaskStatus.Pending);
            task.CreatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateStatus_ShouldChangeStatus()
        {
            var task = new TaskItem(1, "A", "B", null, TaskPriority.Low);

            task.UpdateStatus(TaskStatus.InProgress);

            task.Status.Should().Be(TaskStatus.InProgress);
            task.UpdatedAt.Should().NotBeNull();
            task.CompletedAt.Should().BeNull();
        }

        [Fact]
        public void UpdateStatus_ToCompleted_ShouldSetCompletedAt()
        {
            var task = new TaskItem(1, "A", "B", null, TaskPriority.Low);

            task.UpdateStatus(TaskStatus.Completed);

            task.Status.Should().Be(TaskStatus.Completed);
            task.CompletedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateStatus_ShouldNotChangeCreatedAt()
        {
            var task = new TaskItem(1, "A", "B", null, TaskPriority.Low);

            var createdAt = task.CreatedAt;

            task.UpdateStatus(TaskStatus.InProgress);

            task.CreatedAt.Should().Be(createdAt);
        }
    }
}
