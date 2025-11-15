using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
//using TaskManagement.Api.Controllers;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Tests.Fakes;
using Xunit;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Api.Controllers
{
    public class TaskControllerTests
    {
        private TaskController CreateController(FakeTaskItemRepository taskRepo,
                                                FakeProjectRepository projectRepo,
                                                FakeTaskCommentRepository commentRepo,
                                                FakeTaskHistoryRepository historyRepo)
        {
            var createHandler = new CreateTaskItemCommandHandler(taskRepo, projectRepo);
            var getByIdHandler = new GetTaskItemByIdQueryHandler(taskRepo);
            var updateHandler = new UpdateTaskCommandHandler(taskRepo, historyRepo);
            var deleteHandler = new DeleteTaskCommandHandler(projectRepo, taskRepo);
            //var addCommentHandler = new AddTaskCommentCommandHandler(taskRepo, commentRepo, historyRepo);
            var addCommentHandler = new AddTaskCommentCommandHandler(commentRepo, historyRepo);
            var commentsByTaskHandler = new GetTasksCommentsByTaskIdQueryHandler(commentRepo);
            var commentsAndHistoryHandler = new GetTasksCommentsAndHistoryByTaskIdQueryHandler(commentRepo, historyRepo);

            //return new TaskController(
            //    createHandler,
            //    getByIdHandler,
            //    updateHandler,
            //    deleteHandler,
            //    addCommentHandler,
            //    commentsByTaskHandler,
            //    commentsAndHistoryHandler);

            return new TaskController(
                createHandler,
                getByIdHandler,
                updateHandler,
                addCommentHandler,
                commentsByTaskHandler,
                commentsAndHistoryHandler,
                deleteHandler
                );
        }

        [Fact]
        public async Task CreateTask_ShouldReturnCreated()
        {
            var project = new Project("P", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var taskRepo = new FakeTaskItemRepository();
            var projectRepo = new FakeProjectRepository(new[] { project });
            var commentRepo = new FakeTaskCommentRepository();
            var historyRepo = new FakeTaskHistoryRepository();
            var controller = CreateController(taskRepo, projectRepo, commentRepo, historyRepo);

            var cmd = new CreateTaskItemCommand
            {
                ProjectId = project.Id,
                Title = "Nova Task",
                Description = "Desc",
                Priority = TaskPriority.Low
            };

            var result = await controller.CreateTask(cmd);

            var created = Assert.IsType<StatusCodeResult>(result);
            created.StatusCode.Should().Be(201);

            var tasks = await taskRepo.GetByProjectIdAsync(project.Id);
            tasks.Should().ContainSingle(t => t.Title == "Nova Task");
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnTask()
        {
            var project = new Project("P", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Low);
            typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(task, 1);

            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var controller = CreateController(taskRepo,
                new FakeProjectRepository(new[] { project }),
                new FakeTaskCommentRepository(),
                new FakeTaskHistoryRepository());

            var result = await controller.GetTask(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = ok.Value;
            dto.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnNoContent()
        {
            var project = new Project("P", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Low);
            typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(task, 1);

            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var historyRepo = new FakeTaskHistoryRepository();

            var controller = CreateController(taskRepo,
                new FakeProjectRepository(new[] { project }),
                new FakeTaskCommentRepository(),
                historyRepo);

            var cmd = new UpdateTaskCommand
            {
                TaskId = 1,
                Title = "Atualizada",
                Description = "Nova Desc",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Low
            };

            var result = await controller.UpdateTask(cmd.TaskId, new Application.Handlers.UpdateTaskCommand { Id = cmd.TaskId, Title = cmd.Title, Description = cmd.Description, Status = cmd.Status, Priority = cmd.Priority } );

            Assert.IsType<NoContentResult>(result);

            var updated = await taskRepo.GetByIdAsync(1);
            updated.Title.Should().Be("Atualizada");
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnNoContent()
        {
            var project = new Project("P", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Low);
            typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(task, 1);

            task = new TaskItem { };

            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var controller = CreateController(taskRepo,
                new FakeProjectRepository(new[] { project }),
                new FakeTaskCommentRepository(),
                new FakeTaskHistoryRepository());

            var result = await controller.RemoveTask(task.Id, project.Id);

            Assert.IsType<NoContentResult>(result);

            var all = await taskRepo.GetAllAsync();
            all.Should().BeEmpty();
        }

        [Fact]
        public async Task AddComment_ShouldReturnNoContent_AndSaveComment()
        {
            var project = new Project("P", "Desc", 1);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, 10);

            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Low);
            typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(task, 1);

            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var commentRepo = new FakeTaskCommentRepository();
            var historyRepo = new FakeTaskHistoryRepository();

            var controller = CreateController(taskRepo,
                new FakeProjectRepository(new[] { project }),
                commentRepo,
                historyRepo);

            var cmd = new AddTaskCommentCommand
            {
                TaskItemId = 1,
                Message = "Comentário",
                UserId = 7
            };

            var result = await controller.AddComment(cmd.TaskItemId, new AddTaskCommentRequest { Message  = cmd.Message});

            Assert.IsType<NoContentResult>(result);

            var comments = await commentRepo.GetByTaskIdAsync(1);
            comments.Should().HaveCount(1);
        }
    }
}
