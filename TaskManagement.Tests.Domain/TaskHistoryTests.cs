using FluentAssertions;
using System;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.Tests.Domain
{
    public class TaskHistoryTests
    {
        [Fact]
        public void Should_Create_TaskHistory_With_ValidData()
        {
            var now = DateTime.UtcNow;

            var history = new TaskHistory
            {
                Id = 1,
                TaskItemId = 20,
                Field = "Status",
                ModifiedByUserId = 2,
                ModifiedAt = now,
                ChangedAt = now.AddMinutes(-5),
                ChangedByUserId = 5
            };

            history.Id.Should().Be(1);
            history.TaskItemId.Should().Be(20);
            history.Field.Should().Be("Status");
            history.ModifiedByUserId.Should().Be(2);
            history.ModifiedAt.Should().Be(now);
            history.ChangedAt.Should().Be(now.AddMinutes(-5));
            history.ChangedByUserId.Should().Be(5);
        }

        [Fact]
        public void TaskItem_CanBeNull()
        {
            var history = new TaskHistory
            {
                TaskItem = null
            };

            history.TaskItem.Should().BeNull();
        }

        [Fact]
        public void Should_Associate_With_TaskItem()
        {
            var task = new TaskItem(1, "Título", "Desc", null, TaskPriority.Low);

            var history = new TaskHistory
            {
                TaskItem = task,
                TaskItemId = task.Id
            };

            history.TaskItem.Should().Be(task);
        }
    }
}
