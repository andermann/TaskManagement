
using Microsoft.AspNetCore.Mvc;
//using TaskManagement.Api.Controllers;
using TaskManagement.Infrastructure;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries;  // onde ficam ProjectListDto, GetProjectsByOwnerIdQuery, etc.
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;


namespace TaskManagement.Tests.Api.Controllers
{
    public class ProjectControllerTests
    {
        #region Helpers / Fakes

        //private sealed class InMemoryProjectRepository : IProjectRepository
        //{
        //    private readonly List<Project> _projects;

        //    public int AddCalls { get; private set; }
        //    public int RemoveCalls { get; private set; }

        //    public InMemoryProjectRepository(IEnumerable<Project>? seed = null)
        //    {
        //        _projects = seed?.ToList() ?? new List<Project>();
        //    }

        //    public Task AddAsync(Project project)
        //    {
        //        AddCalls++;
        //        // simulando Id auto-increment
        //        if (project.GetType().GetProperty("Id")?.CanWrite == true)
        //        {
        //            var nextId = _projects.Count == 0 ? 1 : _projects.Max(p => p.Id) + 1;
        //            typeof(Project).GetProperty("Id")!.SetValue(project, nextId);
        //        }

        //        _projects.Add(project);
        //        return Task.CompletedTask;
        //    }

        //    public Task<IEnumerable<Project>> GetAllAsync()
        //        => Task.FromResult<IEnumerable<Project>>(_projects);

        //    public Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(int ownerId)
        //        => Task.FromResult<IEnumerable<Project>>(_projects.Where(p => p.OwnerId == ownerId));

        //    public Task<IEnumerable<Project>> GetProjectsByIdAsync(int id)
        //        => Task.FromResult<IEnumerable<Project>>(_projects.Where(p => p.Id == id));

        //    public Task<Project?> GetByIdAsync(int id)
        //        => Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

        //    public Task RemoveAsync(int id)
        //    {
        //        RemoveCalls++;
        //        var proj = _projects.FirstOrDefault(p => p.Id == id);
        //        if (proj != null)
        //            _projects.Remove(proj);

        //        return Task.CompletedTask;
        //    }

        //    Task<List<Project>> IProjectRepository.GetProjectsByOwnerIdAsync(int ownerId)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task<List<Project>> IProjectRepository.GetAllAsync()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task<int> IProjectRepository.GetTaskCountAsync(int projectId)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task IProjectRepository.UpdateAsync(Project project)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    Task<List<Project>> IProjectRepository.GetProjectsByIdAsync(int ownerId)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    // Caso a interface tenha outros métodos, implemente aqui
        //}

        private sealed class InMemoryProjectRepository : IProjectRepository
        {
            private readonly List<Project> _projects;

            public int AddCalls { get; private set; }
            public int RemoveCalls { get; private set; }

            public InMemoryProjectRepository(IEnumerable<Project>? seed = null)
            {
                _projects = seed?.ToList() ?? new List<Project>();
            }

            public Task AddAsync(Project project)
            {
                AddCalls++;

                // simula auto-incremento
                var nextId = _projects.Count == 0 ? 1 : _projects.Max(p => p.Id) + 1;
                typeof(Project).GetProperty("Id")!.SetValue(project, nextId);

                _projects.Add(project);
                return Task.CompletedTask;
            }

            public Task<List<Project>> GetAllAsync()
                => Task.FromResult(_projects.ToList());

            public Task<List<Project>> GetProjectsByOwnerIdAsync(int ownerId)
                => Task.FromResult(_projects.Where(p => p.OwnerId == ownerId).ToList());

            public Task<List<Project>> GetProjectsByIdAsync(int id)
                => Task.FromResult(_projects.Where(p => p.Id == id).ToList());

            public Task<Project?> GetByIdAsync(int id)
                => Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

            public Task<int> GetTaskCountAsync(int projectId)
            {
                // se você não usa isso em nenhum handler, pode devolver 0 mesmo
                return Task.FromResult(0);
            }

            public Task UpdateAsync(Project project)
            {
                var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
                if (existing != null)
                {
                    existing.Title = project.Title;
                    existing.Description = project.Description;
                    // ajuste conforme seus métodos da entidade Project
                }
                return Task.CompletedTask;
            }

            public Task RemoveAsync(int id)
            {
                RemoveCalls++;
                var proj = _projects.FirstOrDefault(p => p.Id == id);
                if (proj != null)
                    _projects.Remove(proj);

                return Task.CompletedTask;
            }
        }


        private sealed class InMemoryTaskItemRepository : ITaskItemRepository
        {
            public bool HasPending { get; set; }

            public Task<bool> HasPendingTasksAsync(int projectId)
                => Task.FromResult(HasPending);

            Task ITaskItemRepository.AddAsync(TaskItem task)
            {
                throw new NotImplementedException();
            }

            Task<int> ITaskItemRepository.CountByProjectAsync(int projectId)
            {
                throw new NotImplementedException();
            }

            Task<int> ITaskItemRepository.CountTasksByProjectIdAsync(int projectId)
            {
                throw new NotImplementedException();
            }

            Task<List<TaskItem>> ITaskItemRepository.GetAllAsync()
            {
                throw new NotImplementedException();
            }

            Task<TaskItem> ITaskItemRepository.GetByIdAsync(int taskId)
            {
                throw new NotImplementedException();
            }

            Task<List<TaskItem>> ITaskItemRepository.GetByProjectIdAsync(int projectId)
            {
                throw new NotImplementedException();
            }

            Task ITaskItemRepository.RemoveAsync(TaskItem task)
            {
                throw new NotImplementedException();
            }

            Task ITaskItemRepository.UpdateAsync(TaskItem task)
            {
                throw new NotImplementedException();
            }

            // coloque outros métodos da interface, se existirem, jogando NotImplementedException
        }

        private static ProjectController CreateController(
            InMemoryProjectRepository projRepo,
            InMemoryTaskItemRepository? taskRepo = null)
        {
            var createHandler = new CreateProjectCommandHandler(projRepo);
            var listByOwner = new GetProjectsByOwnerIdQueryHandler(projRepo);
            var listAll = new GetProjectsAllQueryHandler(projRepo);
            var listById = new GetProjectsByIdQueryHandler(projRepo);
            var deleteHandler = new DeleteProjectCommandHandler(projRepo, taskRepo ?? new InMemoryTaskItemRepository());
            GetTasksByProjectIdQueryHandler? tasksByProjectHandler = null!; // não será usado nesses testes

            return new ProjectController(
                createHandler,
                listByOwner,
                listAll,
                listById,
                deleteHandler,
                tasksByProjectHandler
            );
        }

        private static Project NewProject(int id, int ownerId, string title = "P", string desc = "D")
        {
            var p = new Project(title, desc, ownerId);
            // setar Id via reflexão, porque a propriedade é privada set
            typeof(Project).GetProperty("Id")!.SetValue(p, id);
            return p;
        }

        #endregion

        #region Controller tests

        [Fact]
        public async Task ListProjects_ReturnsOkWithUserProjects()
        {
            // arrange
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "Meu projeto", "Desc"),
                NewProject(2, 2, "Outro projeto", "Desc")
            });
            var controller = CreateController(repo);

            // act
            var result = await controller.ListProjects();

            // assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProjectListDto>>(ok.Value);
            Assert.Single(model);                 // apenas ownerId = 1
            Assert.Equal("Meu projeto", model.First().Title);
        }

        [Fact]
        public async Task ListAllProjects_ReturnsOkWithAllProjects()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1"),
                NewProject(2, 2, "P2", "D2")
            });
            var controller = CreateController(repo);

            var result = await controller.ListAllProjects();

            var ok = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Project>>(ok.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetProject_ReturnsOkWithProjectList()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1")
            });
            var controller = CreateController(repo);

            var result = await controller.GetProject(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<ProjectListDto>>(ok.Value);
            Assert.Single(list);
            Assert.Equal(1, list.First().Id);
        }

        [Fact]
        public async Task CreateProject_ReturnsCreatedAndCallsHandler()
        {
            var repo = new InMemoryProjectRepository();
            var controller = CreateController(repo);

            var cmd = new CreateProjectCommand
            {
                Title = "Novo",
                Description = "Desc",
                OwnerId = 1
            };

            var result = await controller.CreateProject(cmd);

            var created = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, created.StatusCode);
            Assert.Equal(1, repo.AddCalls);
        }

        [Fact]
        public async Task DeleteProject_WhenSuccess_ReturnsNoContent()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1")
            });
            var taskRepo = new InMemoryTaskItemRepository { HasPending = false };
            var controller = CreateController(repo, taskRepo);

            var result = await controller.DeleteProject(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, repo.RemoveCalls);
        }

        [Fact]
        public async Task DeleteProject_WhenHasPendingTasks_ReturnsBadRequest()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1")
            });
            var taskRepo = new InMemoryTaskItemRepository { HasPending = true };
            var controller = CreateController(repo, taskRepo);

            var result = await controller.DeleteProject(1);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var body = bad.Value as dynamic;
            Assert.Contains("tarefas pendentes", bad.Value.ToString());
        }

        [Fact]
        public async Task DeleteProject_WhenProjectNotFound_ReturnsNotFound()
        {
            var repo = new InMemoryProjectRepository(); // vazio, não tem projeto
            var taskRepo = new InMemoryTaskItemRepository { HasPending = false };
            var controller = CreateController(repo, taskRepo);

            var result = await controller.DeleteProject(123);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Handlers tests

        [Fact]
        public async Task CreateProjectCommandHandler_AddsProjectToRepository()
        {
            var repo = new InMemoryProjectRepository();
            var handler = new CreateProjectCommandHandler(repo);

            var cmd = new CreateProjectCommand
            {
                Title = "Novo",
                Description = "Desc",
                OwnerId = 1
            };

            await handler.Handle(cmd);

            Assert.Equal(1, repo.AddCalls);
        }

        [Fact]
        public async Task DeleteProjectCommandHandler_WhenProjectNotFound_ThrowsException()
        {
            var repo = new InMemoryProjectRepository();
            var taskRepo = new InMemoryTaskItemRepository { HasPending = false };
            var handler = new DeleteProjectCommandHandler(repo, taskRepo);

            await Assert.ThrowsAsync<System.Exception>(() =>
                handler.Handle(new DeleteProjectCommand { ProjectId = 99 }));
        }

        [Fact]
        public async Task DeleteProjectCommandHandler_WhenHasPendingTasks_ThrowsInvalidOperation()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1")
            });
            var taskRepo = new InMemoryTaskItemRepository { HasPending = true };
            var handler = new DeleteProjectCommandHandler(repo, taskRepo);

            await Assert.ThrowsAsync<System.InvalidOperationException>(() =>
                handler.Handle(new DeleteProjectCommand { ProjectId = 1 }));
        }

        [Fact]
        public async Task DeleteProjectCommandHandler_WhenNoPendingTasks_RemovesProject()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1")
            });
            var taskRepo = new InMemoryTaskItemRepository { HasPending = false };
            var handler = new DeleteProjectCommandHandler(repo, taskRepo);

            await handler.Handle(new DeleteProjectCommand { ProjectId = 1 });

            Assert.Equal(1, repo.RemoveCalls);
        }

        [Fact]
        public async Task GetProjectsAllQueryHandler_ReturnsAllProjects()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1"),
                NewProject(2, 2, "P2", "D2")
            });
            var handler = new GetProjectsAllQueryHandler(repo);

            var result = await handler.Handle();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProjectsByOwnerIdQueryHandler_ReturnsDtoList()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1"),
                NewProject(2, 2, "P2", "D2")
            });
            var handler = new GetProjectsByOwnerIdQueryHandler(repo);

            var list = await handler.Handle(new GetProjectsByOwnerIdQuery { OwnerId = 1 });

            Assert.Single(list);
            Assert.Equal(1, list.First().Id);
            Assert.Equal("P1", list.First().Title);
        }

        [Fact]
        public async Task GetProjectsByIdQueryHandler_ReturnsDtoList()
        {
            var repo = new InMemoryProjectRepository(new[]
            {
                NewProject(1, 1, "P1", "D1"),
                NewProject(2, 1, "P2", "D2")
            });
            var handler = new GetProjectsByIdQueryHandler(repo);

            var list = await handler.Handle(new GetProjectsByIdQuery { Id = 2 });

            Assert.Single(list);
            Assert.Equal(2, list.First().Id);
        }

        #endregion

        #region Project entity tests

        //[Fact]
        //public void Project_CheckTaskLimit_Throws_WhenMoreThanMax()
        //{
        //    var p = new Project("P", "D", 1);

        //    // injeta 20 tarefas na lista privada via reflexão
        //    var taskListField = typeof(Project).GetField("_tasks",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        //    var tasks = new List<TaskItem>();
        //    for (int i = 0; i < Project.MaxTasks; i++)
        //    {
        //        //var t = new TaskItem("T" + i, "D", 1, 1, 1);
        //        var t = new TaskItem(i, "T", "D", DateTime.Now , 0);
        //        /*
        //         public TaskItem(int projectId, string title, string description, DateTime? dueDate, TaskPriority priority)
        //{
        //    ProjectId = projectId;
        //    Title = title;
        //    Description = description;
        //    DueDate = dueDate;
        //    Status = Enums.TaskStatus.Pending; // Status inicial
        //    Priority = priority;

        //}
        //         */
        //        tasks.Add(t);
        //    }
        //    taskListField.SetValue(p, tasks);

        //    Assert.Throws<InvalidOperationException>(() => p.CheckTaskLimit());
        //}

        //[Fact]
        //public void Project_HasPendingTasks_ReturnsTrue_WhenHasPendingOrInProgress()
        //{
        //    var p = new Project("P", "D", 1);

        //    var taskListField = typeof(Project).GetField("_tasks",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        //    var tasks = new List<TaskItem>
        //    {
        //        new TaskItem(1, "D", "D", DateTime.Now, 0) ,
        //        new TaskItem(2, "D",  "D",DateTime.Now, 0) ,
        //    };
        //    taskListField.SetValue(p, tasks);

        //    Assert.True(p.HasPendingTasks());
        //}

        //[Fact]
        //public void Project_HasPendingTasks_ReturnsFalse_WhenAllDone()
        //{
        //    var p = new Project("P", "D", 1);

        //    var taskListField = typeof(Project).GetField("_tasks",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        //    var tasks = new List<TaskItem>
        //    {
        //        new TaskItem(1, "T1", "D", DateTime.Now, TaskPriority.Low),
        //        new TaskItem(1, "T1", "D", DateTime.Now, TaskPriority.Low),
        //    };
        //    taskListField.SetValue(p, tasks);

        //    Assert.False(p.HasPendingTasks());
        //}

        #endregion
    }
}

