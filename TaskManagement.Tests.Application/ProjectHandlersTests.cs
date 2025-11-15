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
    public class ProjectHandlersTests
    {
        private static Project NewProject(int id, int ownerId, string title)
        {
            var p = new Project(title, "Desc", ownerId);
            typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(p, id);
            return p;
        }

        [Fact]
        public async Task CreateProjectCommandHandler_ShouldAddProject()
        {
            var repo = new FakeProjectRepository();
            var handler = new CreateProjectCommandHandler(repo);

            var cmd = new CreateProjectCommand
            {
                Title = "Novo Projeto",
                Description = "Teste",
                OwnerId = 1
            };

            await handler.Handle(cmd);

            var all = await repo.GetAllAsync();
            all.Should().ContainSingle(p => p.Title == "Novo Projeto" && p.OwnerId == 1);
        }

        [Fact]
        public async Task GetProjectsAllQueryHandler_ShouldReturnAllProjects()
        {
            var seed = new[]
            {
                NewProject(1, 1, "P1"),
                NewProject(2, 2, "P2")
            };
            var repo = new FakeProjectRepository(seed);
            var handler = new GetProjectsAllQueryHandler(repo);

            //var result = await handler.Handle(new GetProjectsAllQuery());
            var result = await handler.Handle();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProjectsByOwnerIdQueryHandler_ShouldReturnProjectsForOwner()
        {
            var seed = new[]
            {
                NewProject(1, 1, "P1"),
                NewProject(2, 2, "P2")
            };
            var repo = new FakeProjectRepository(seed);
            var handler = new GetProjectsByOwnerIdQueryHandler(repo);

            var result = await handler.Handle(new GetProjectsByOwnerIdQuery { OwnerId = 1 });

            result.Should().HaveCount(1);
            result.First().Title.Should().Be("P1");
            result.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetProjectsByIdQueryHandler_ShouldReturnMatchingProject()
        {
            var seed = new[]
            {
                NewProject(1, 1, "P1"),
                NewProject(2, 1, "P2")
            };
            var repo = new FakeProjectRepository(seed);
            var handler = new GetProjectsByIdQueryHandler(repo);

            //var result = await handler.Handle(new GetProjectsByIdQuery { Id = 2 });

            //result.Should().ContainSingle(p => p.Id == 2 && p.Title == "P2");
            
            // Se o Handler retornar um único objeto (ProjectDto)
            var result = await handler.Handle(new GetProjectsByIdQuery { Id = 2 });
            result.Should().NotBeNull();
            result.FirstOrDefault().Id.Should().Be(2);
            result.FirstOrDefault().Title.Should().Be("P2");

        }

        [Fact]
        public async Task DeleteProjectCommandHandler_ShouldThrow_WhenProjectNotFound()
        {
            var projectRepo = new FakeProjectRepository();
            var taskRepo = new FakeTaskItemRepository();
            var handler = new DeleteProjectCommandHandler(projectRepo, taskRepo);

            var cmd = new DeleteProjectCommand { ProjectId = 99 };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(cmd));
        }

        [Fact]
        public async Task DeleteProjectCommandHandler_ShouldThrow_WhenHasPendingTasks()
        {
            var project = NewProject(1, 1, "P1");
            var projectRepo = new FakeProjectRepository(new[] { project });

            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Medium);
            var taskRepo = new FakeTaskItemRepository(new[] { task });
            var handler = new DeleteProjectCommandHandler(projectRepo, taskRepo);

            var cmd = new DeleteProjectCommand { ProjectId = project.Id };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd));
        }

        [Fact]
        public async Task DeleteProjectCommandHandler_ShouldRemoveProject_WhenNoPendingTasks()
        {
            var project = NewProject(1, 1, "P1");
            var projectRepo = new FakeProjectRepository(new[] { project });

            // todas as tasks completadas
            var task = new TaskItem(project.Id, "T1", "Desc", null, TaskPriority.Medium);
            task.UpdateStatus(TaskStatus.Completed);
            var taskRepo = new FakeTaskItemRepository(new[] { task });

            var handler = new DeleteProjectCommandHandler(projectRepo, taskRepo);

            await handler.Handle(new DeleteProjectCommand { ProjectId = project.Id });

            var all = await projectRepo.GetAllAsync();
            all.Should().BeEmpty();
        }
    }
}
