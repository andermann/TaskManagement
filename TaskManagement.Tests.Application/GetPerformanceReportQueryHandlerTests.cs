using Moq;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Application
{

    public class GetPerformanceReportQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ManagerWithCompletedTasks_ReturnsExpectedDto()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var tasks = new List<TaskItem>
            {
                new TaskItem(1, "T1", "d", now.AddDays(30), TaskPriority.Medium)
                {
                    Id = 1,
                    CreatedAt = now.AddDays(-10),
                    CompletedAt = now.AddDays(-5),
                    CreatedByUserId = 1,
                    Status = TaskStatus.Completed
                },
                new TaskItem(1, "T2", "d", now.AddDays(30), TaskPriority.Medium)
                {
                    Id = 2,
                    CreatedAt = now.AddDays(-20),
                    CompletedAt = now.AddDays(-15),
                    CreatedByUserId = 1,
                    Status = TaskStatus.Completed
                },
                new TaskItem(1, "T3", "d", now.AddDays(30), TaskPriority.Medium)
                {
                    Id = 3,
                    CreatedAt = now.AddDays(-8),
                    CompletedAt = now.AddDays(-3),
                    CreatedByUserId = 2,
                    Status = TaskStatus.Completed
                },
                new TaskItem(1, "T4", "d", now.AddDays(30), TaskPriority.Low)
                {
                    Id = 4,
                    CreatedAt = now.AddDays(-2),
                    CompletedAt = null,
                    CreatedByUserId = 1,
                    Status = TaskStatus.Pending
                }
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var manager = new User(1, "Manager", "m@x.com", "Manager");
            mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(manager);

            var mockTaskRepo = new Mock<ITaskItemRepository>();
            mockTaskRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            var handler = new GetPerformanceReportQueryHandler(mockUserRepo.Object, mockTaskRepo.Object);

            // Act
            var dto = await handler.Handle(new GetPerformanceReportQuery { UserId = 1 });

            // Assert
            Assert.Equal("Manager", dto.ReportGeneratedBy);
            // completed tasks for userId 1 within last 30 days = T1 and T2 => 2
            Assert.Equal(2, dto.CompletedTasksLast30Days);
            // average completion time for those two tasks = (5 + 5) / 2 = 5.00
            Assert.Equal(5.00, dto.AverageCompletionTimeDays);
            // CompletedTasks = total completed across all users in last 30 days = T1, T2, T3 => 3
            Assert.Equal(3, dto.CompletedTasks);
            // AvgCompletionDays (int cast) = average of all completed tasks durations = 5
            Assert.Equal(5, dto.AvgCompletionDays);
        }

        [Fact]
        public async Task Handle_NonManager_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var nonManager = new User(2, "User", "u@x.com", "User");
            mockUserRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(nonManager);

            var mockTaskRepo = new Mock<ITaskItemRepository>();
            mockTaskRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem>());

            var handler = new GetPerformanceReportQueryHandler(mockUserRepo.Object, mockTaskRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new GetPerformanceReportQuery { UserId = 2 }));
        }
    }
}