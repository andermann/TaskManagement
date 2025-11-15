using System;
using FluentAssertions;
using TaskManagement.Domain.Entities;
using Xunit;

namespace TaskManagement.Tests.Domain
{
    public class TaskCommentTests
    {
        [Fact]
        public void Should_Create_TaskComment_With_ValidData()
        {
            var now = DateTime.UtcNow;

            var comment = new TaskComment
            {
                Id = 10,
                TaskItemId = 5,
                Message = "Mensagem de teste",
                CreatedAt = now,
                CreatedByUserId = 99
            };

            comment.Id.Should().Be(10);
            comment.TaskItemId.Should().Be(5);
            comment.Message.Should().Be("Mensagem de teste");
            comment.CreatedAt.Should().Be(now);
            comment.CreatedByUserId.Should().Be(99);
        }

        [Fact]
        public void Message_Can_Be_Null()
        {
            var comment = new TaskComment
            {
                Message = null
            };

            comment.Message.Should().BeNull();
        }
    }
}
