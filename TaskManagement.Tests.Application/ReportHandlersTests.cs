using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Tests.Fakes;
using Xunit;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Application
{
    public class ReportHandlersTests
    {
        [Fact]
        public async Task GetPerformanceReportQueryHandler_ShouldReturnReport()
        {

            var user = new User(1, "Gerente", "manager@test.com", "manager");
            //typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, 1);

            var userRepo = new FakeUserRepository(new[] { user });
            var tasks = new List<TaskItem>
            {
                new TaskItem(1, "T1", "Desc", DateTime.UtcNow.AddDays(-2), TaskPriority.Low)
                    { Status = TaskStatus.Completed, 
                    CreatedAt = DateTime.UtcNow.AddDays(-5), 
                    CompletedAt = DateTime.UtcNow.AddDays(-1), 
                    CreatedByUserId = user.Id
                },
                new TaskItem(1, "T2", "Desc", DateTime.UtcNow.AddDays(1), TaskPriority.Medium)
                    { Status = TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    CompletedAt = null,
                    CreatedByUserId = user.Id
                }
            };

            var taskRepo = new FakeTaskItemRepository(tasks);
            var handler = new GetPerformanceReportQueryHandler(userRepo, taskRepo);

            var query = new GetPerformanceReportQuery { LastDays = 30 , UserId = user.Id};

            var result = await handler.Handle(query);

            result.Should().NotBeNull();
            result.TotalTasks.Should().Be(2);
            result.CompletedTasks.Should().Be(1);
        }
    }
}
