using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Tests.Fakes;
using Xunit;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Application
{
    public class TaskHandlersTests
    {
        private static TaskItem NewTask(int id, int projectId, TaskStatus status = TaskStatus.Pending)
        {
            var t = new TaskItem(projectId, $"Task {id}", "Desc", null, TaskPriority.Low);
            typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(t, id);
            t.UpdateStatus(status);
            return t;
        }

        [Fact]
        public async Task CreateTaskItemCommandHandler_ShouldAddTask()
        {
            var project = new Project("P1", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var projectRepo = new FakeProjectRepository(new[] { project });
            var taskRepo = new FakeTaskItemRepository();
            var handler = new CreateTaskItemCommandHandler(taskRepo, projectRepo);

            var cmd = new CreateTaskItemCommand
            {
                ProjectId = project.Id,
                Title = "Nova tarefa",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(3),
                Priority = TaskPriority.High
            };

            await handler.Handle(cmd);

            var tasks = await taskRepo.GetByProjectIdAsync(project.Id);
            tasks.Should().ContainSingle(t => t.Title == "Nova tarefa");
        }

        [Fact]
        public async Task GetTaskItemByIdQueryHandler_ShouldReturnTask()
        {
            var task = NewTask(1, 99);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var handler = new GetTaskItemByIdQueryHandler(taskRepo);

            var result = await handler.Handle(new GetTaskItemByIdQuery { TaskId = 1 });

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetTasksByProjectIdQueryHandler_ShouldReturnTasksForProject()
        {
            var t1 = NewTask(1, 10);
            var t2 = NewTask(2, 20);
            var task = NewTask(1, 10);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var taskItemRepo = new FakeTaskItemRepository(new[] { t1, t2 });

            var project = new Project("P1", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var projectRepo = new FakeProjectRepository(new[] { project });
            var handler = new GetTasksByProjectIdQueryHandler(projectRepo, taskRepo);

            var result = await handler.Handle(new GetTasksByProjectIdQuery { ProjectId = 10 });

            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateTaskCommandHandler_ShouldUpdateFields_AndAddHistory()
        {
            var task = NewTask(1, 10, TaskStatus.Pending);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var historyRepo = new FakeTaskHistoryRepository();
            var handler = new UpdateTaskCommandHandler(taskRepo, historyRepo);

            var cmd = new UpdateTaskCommand
            {
                TaskId = 1,
                Title = "Atualizada",
                Description = "Nova desc",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Low,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            await handler.Handle(new TaskManagement.Application.Handlers.UpdateTaskCommand { Id = cmd.TaskId, Title = cmd.Title, Description = cmd.Description, Status = cmd.Status, Priority = cmd.Priority });

            var updated = await taskRepo.GetByIdAsync(1);
            updated.Title.Should().Be("Atualizada");
            updated.Status.Should().Be(TaskStatus.Completed);

            var history = await historyRepo.GetByTaskIdAsync(1);
            history.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteTaskCommandHandler_ShouldRemoveTask_AndRegisterHistory()
        {

            var project = new Project("Projeto 1", "Desc", ownerId: 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);
            var projectRepo = new FakeProjectRepository(new[] { project });

            var task = NewTask(10, 10, TaskStatus.InProgress);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var historyRepo = new FakeTaskHistoryRepository();
            //var handler = new DeleteTaskCommandHandler(taskRepo, historyRepo);
            var handler = new DeleteTaskCommandHandler(projectRepo, taskRepo);

            await handler.Handle(new DeleteTaskCommand { TaskId = task.Id });

            var all = await taskRepo.GetAllAsync();
            all.Should().BeEmpty();

            var history = await historyRepo.GetByTaskIdAsync(task.Id);
            history.Should().BeEmpty();
        }

        [Fact]
        public async Task AddTaskCommentCommandHandler_ShouldAddComment_AndHistory()
        {
            var task = NewTask(1, 10);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var commentRepo = new FakeTaskCommentRepository();
            var historyRepo = new FakeTaskHistoryRepository();
            //var handler = new AddTaskCommentCommandHandler(taskRepo, commentRepo, historyRepo);
            var handler = new AddTaskCommentCommandHandler(commentRepo, historyRepo);

            var cmd = new AddTaskCommentCommand
            {
                TaskItemId = 1,
                Message = "Comentário teste",
                UserId = 5
            };

            await handler.Handle(cmd);

            var comments = await commentRepo.GetByTaskIdAsync(1);
            comments.Should().ContainSingle(c => c.Message == "Comentário teste");

            var history = await historyRepo.GetByTaskIdAsync(1);
            history.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTasksCommentsByTaskIdQueryHandler_ShouldReturnComments()
        {
            var comments = new List<TaskComment>
            {
                new TaskComment { TaskItemId = 1, Message = "A" },
                new TaskComment { TaskItemId = 2, Message = "B" }
            };
            var repo = new FakeTaskCommentRepository(comments);
            var handler = new GetTasksCommentsByTaskIdQueryHandler(repo);

            var result = await handler.Handle(new GetTasksCommentsByTaskIdQuery { TaskItemId = 1 });

            result.Should().HaveCount(1);
            result.First().Message.Should().Be("A");
        }

        [Fact]
        public async Task GetTasksCommentsAndHistoryByTaskIdQueryHandler_ShouldReturnCombined()
        {
            var comments = new List<TaskComment>
            {
                new TaskComment { TaskItemId = 1, Message = "C1" }
            };
            var history = new List<TaskHistory>
            {
                new TaskHistory { TaskItemId = 1, Field = "Status" }
            };

            var commentRepo = new FakeTaskCommentRepository(comments);
            var historyRepo = new FakeTaskHistoryRepository(history);
            var handler = new GetTasksCommentsAndHistoryByTaskIdQueryHandler(commentRepo, historyRepo);

            var result = await handler.Handle(new GetTasksCommentsAndHistoryByTaskIdQuery { TaskItemId = 1 });

            result.Comments.Should().HaveCount(1);
            result.History.Should().HaveCount(1);
        }
    }
}
