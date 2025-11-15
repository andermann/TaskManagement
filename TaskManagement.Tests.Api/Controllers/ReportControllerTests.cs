using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using TaskManagement.Api.Controllers;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Tests.Fakes;
using Xunit;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Api.Controllers
{
    public class ReportControllerTests
    {
        [Fact]
        public async Task GetPerformanceReport_ShouldReturnOkWithReport()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem(1, "T1", "Desc", DateTime.UtcNow.AddDays(-1), TaskPriority.Low)
                {
                    Status = TaskStatus.Completed, CreatedAt = DateTime.UtcNow.AddDays(+5), 
                    CompletedAt = DateTime.UtcNow.AddDays(+3)
                },
                new TaskItem(1, "T2", "Desc", DateTime.UtcNow.AddDays(3), TaskPriority.High)
                {
                    Status = TaskStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(+6), 
                    CompletedAt = DateTime.UtcNow.AddDays(+3)
                }
            };

            var taskRepo = new FakeTaskItemRepository(tasks);

            var user = new User(1, "Gerente", "manager@test.com", "manager");
            var userRepo = new FakeUserRepository(new[] { user });

            var handler = new GetPerformanceReportQueryHandler(userRepo, taskRepo);
            var controller = new ReportController(handler);

            //var result = await controller.GetPerformanceReport(new GetPerformanceReportQuery { LastDays = 30 });
            var result = await controller.GetPerformanceReport();

            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().NotBeNull();
        }
    }
}
