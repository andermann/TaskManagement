//using TaskManagement.Application.Interfaces;
//using TaskManagement.Domain.Entities;

//namespace TaskManagement.Tests.Fakes
//{
//    public class FakeProjectRepository : IProjectRepository
//    {
//        private readonly List<Project> _projects = new();

//        public FakeProjectRepository(IEnumerable<Project>? seed = null)
//        {
//            if (seed != null)
//                _projects.AddRange(seed);
//        }

//        public Task AddAsync(Project project)
//        {
//            project.GetType().GetProperty("Id")!.SetValue(project, _projects.Count + 1);
//            _projects.Add(project);
//            return Task.CompletedTask;
//        }

//        public Task<List<Project>> GetAllAsync()
//            => Task.FromResult(_projects.ToList());

//        public Task<Project?> GetByIdAsync(int id)
//            => Task.FromResult(_projects.FirstOrDefault(x => x.Id == id));

//        public Task<List<Project>> GetProjectsByIdAsync(int id)
//            => Task.FromResult(_projects.Where(x => x.Id == id).ToList());

//        public Task<List<Project>> GetProjectsByOwnerIdAsync(int ownerId)
//            => Task.FromResult(_projects.Where(x => x.OwnerId == ownerId).ToList());

//        public Task RemoveAsync(int id)
//        {
//            _projects.RemoveAll(x => x.Id == id);
//            return Task.CompletedTask;
//        }

//        public Task<int> GetTaskCountAsync(int projectId)
//            => Task.FromResult(0);

//        public Task UpdateAsync(Project project)
//            => Task.CompletedTask;

//        public Task<int> CountTasksByProjectIdAsync(int projectId)
//            => Task.FromResult(0);

//        public Task<bool> HasPendingTasksAsync(int projectId)
//            => Task.FromResult(false);

//        public Task<List<Project>> GetAllSync()
//            => Task.FromResult(_projects.ToList());

//    }

//    public class FakeTaskItemRepository : ITaskItemRepository
//    {
//        private readonly List<TaskItem> _items = new();

//        public FakeTaskItemRepository(IEnumerable<TaskItem>? seed = null)
//        {
//            if (seed != null)
//                _items.AddRange(seed);
//        }

//        public Task AddAsync(TaskItem task)
//        {
//            task.GetType().GetProperty("Id")!.SetValue(task, _items.Count + 1);
//            _items.Add(task);
//            return Task.CompletedTask;
//        }

//        public Task<TaskItem?> GetByIdAsync(int id)
//            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

//        public Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
//            => Task.FromResult(_items.Where(x => x.ProjectId == projectId).ToList());

//        public Task UpdateAsync(TaskItem task)
//        {
//            var index = _items.FindIndex(t => t.Id == task.Id);
//            if (index >= 0)
//                _items[index] = task;
//            return Task.CompletedTask;
//        }

//        public Task RemoveAsync(int id)
//        {
//            _items.RemoveAll(x => x.Id == id);
//            return Task.CompletedTask;
//        }

//        public Task RemoveAsync(TaskItem task)
//        {
//            _items.RemoveAll(x => x.Id == task.Id);
//            return Task.CompletedTask;
//        }

//        public Task<int> CountByProjectAsync(int projectId)
//                => Task.FromResult(_items.Count(x => x.ProjectId == projectId));


//        public Task<int> CountTasksAsync()
//            => Task.FromResult(_items.Count);

//        public Task<int> CountTasksByProjectIdAsync(int projectId)
//            => Task.FromResult(_items.Count(x => x.ProjectId == projectId));

//        public Task<bool> HasPendingTasksAsync(int projectId)
//            => Task.FromResult(_items.Any(x => x.ProjectId == projectId
//                && x.Status != Domain.Enums.TaskStatus.Completed));

//        public Task<List<TaskItem>> GetAllAsync()
//            => Task.FromResult(_items.ToList());

//        public Task<List<TaskItem>> GetByProjectIdAsync(int projectId, int? limit = null)
//        {
//            var result = _items.Where(x => x.ProjectId == projectId).ToList();
//            if (limit.HasValue)
//                result = result.Take(limit.Value).ToList();
//            return Task.FromResult(result);
//        }


//    }

//    public class FakeTaskCommentRepository : ITaskCommentRepository
//    {
//        public readonly List<TaskComment> Comments = new();

//        public Task AddAsync(TaskComment comment)
//        {
//            Comments.Add(comment);
//            return Task.CompletedTask;
//        }

//        public Task<List<TaskComment>> GetByTaskIdAsync(int taskItemId)
//            => Task.FromResult(Comments.Where(x => x.TaskItemId == taskItemId).ToList());

//        public Task<List<TaskComment>> GetAllAsync()
//                => Task.FromResult(Comments.ToList());

//        public Task AddRangeAsync(IEnumerable<TaskComment> comments)
//        {
//            Comments.AddRange(comments);
//            return Task.CompletedTask;
//        }

//        Task<IEnumerable<TaskComment>> ITaskCommentRepository.GetByTaskIdAsync(int taskItemId)
//                    => Task.FromResult(Comments.Where(x => x.TaskItemId == taskItemId));
//    }

//    public class FakeTaskHistoryRepository : ITaskHistoryRepository
//    {
//        public readonly List<TaskHistory> History = new();

//        public Task AddAsync(TaskHistory history)
//        {
//            History.Add(history);
//            return Task.CompletedTask;
//        }

//        public Task<List<TaskHistory>> GetByTaskIdAsync(int taskItemId)
//            => Task.FromResult(History.Where(x => x.TaskItemId == taskItemId).ToList());

//        public Task AddRangeAsync(IEnumerable<TaskHistory> history)
//        {
//            History.AddRange(history);
//            return Task.CompletedTask;
//        }

//        public Task<List<TaskHistory>> GetAllAsync()
//            => Task.FromResult(History.ToList());

//        Task<IEnumerable<TaskHistory>> ITaskHistoryRepository.GetByTaskIdAsync(int taskId)
//        => Task.FromResult(History.Where(x => x.TaskItemId == taskId));
//    }
//}

using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Fakes
{
    public class FakeProjectRepository : IProjectRepository
    {
        private readonly List<Project> _projects = new();

        public FakeProjectRepository(IEnumerable<Project>? seed = null)
        {
            if (seed != null)
                _projects.AddRange(seed);
        }

        public Task<Project> GetByIdAsync(int projectId)
        {
            // em produção pode ser null; aqui usamos ! para simplificar
            var project = _projects.FirstOrDefault(p => p.Id == projectId)
                          ?? new Project("fake", "fake", 0);
            return Task.FromResult(project);
        }

        public Task<List<Project>> GetProjectsByOwnerIdAsync(int ownerId)
            => Task.FromResult(_projects.Where(p => p.OwnerId == ownerId).ToList());

        public Task<List<Project>> GetAllAsync()
            => Task.FromResult(_projects.ToList());

        public Task<int> GetTaskCountAsync(int projectId)
        {
            // se algum handler usar isso, você pode adaptar a lógica;
            // por enquanto, retornamos 0 por padrão.
            return Task.FromResult(0);
        }

        public Task AddAsync(Project project)
        {
            if (project.Id == 0)
            {
                var nextId = _projects.Count == 0 ? 1 : _projects.Max(p => p.Id) + 1;
                typeof(Project).GetProperty(nameof(Project.Id))!
                    .SetValue(project, nextId);
            }

            _projects.Add(project);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Project project)
        {
            var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existing != null)
            {
                existing.Title = project.Title;
                existing.Description = project.Description;
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(int projectId)
        {
            _projects.RemoveAll(p => p.Id == projectId);
            return Task.CompletedTask;
        }

        public Task<List<Project>> GetProjectsByIdAsync(int Id)
            => Task.FromResult(_projects.Where(p => p.Id == Id).ToList());
    }

    public class FakeTaskItemRepository : ITaskItemRepository
    {
        private readonly List<TaskItem> _tasks = new();

        public FakeTaskItemRepository(IEnumerable<TaskItem>? seed = null)
        {
            if (seed != null)
                _tasks.AddRange(seed);
        }

        public Task<TaskItem> GetByIdAsync(int taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId)
                       ?? new TaskItem(0, "fake", "fake", null, TaskPriority.Low);
            return Task.FromResult(task);
        }

        public Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
            => Task.FromResult(_tasks.Where(t => t.ProjectId == projectId).ToList());

        public Task AddAsync(TaskItem task)
        {
            if (task.Id == 0)
            {
                var nextId = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
                typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!
                    .SetValue(task, nextId);
            }

            _tasks.Add(task);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TaskItem task)
        {
            var index = _tasks.FindIndex(t => t.Id == task.Id);
            if (index >= 0)
                _tasks[index] = task;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(TaskItem task)
        {
            _tasks.RemoveAll(t => t.Id == task.Id);
            return Task.CompletedTask;
        }

        public Task<int> CountTasksByProjectIdAsync(int projectId)
            => Task.FromResult(_tasks.Count(t => t.ProjectId == projectId));

        public Task<int> CountByProjectAsync(int projectId)
            => Task.FromResult(_tasks.Count(t => t.ProjectId == projectId));

        public Task<bool> HasPendingTasksAsync(int projectId)
        {
            var has = _tasks.Any(t => t.ProjectId == projectId &&
                                      t.Status != TaskStatus.Completed);
            return Task.FromResult(has);
        }

        public Task<List<TaskItem>> GetAllAsync()
            => Task.FromResult(_tasks.ToList());
    }

    public class FakeTaskCommentRepository : ITaskCommentRepository
    {
        private readonly List<TaskComment> _comments = new();

        public FakeTaskCommentRepository(IEnumerable<TaskComment>? seed = null)
        {
            if (seed != null)
                _comments.AddRange(seed);
        }

        public Task AddAsync(TaskComment comment)
        {
            _comments.Add(comment);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TaskComment>> GetByTaskIdAsync(int taskItemId)
        {
            IEnumerable<TaskComment> result =
                _comments.Where(c => c.TaskItemId == taskItemId).ToList();
            return Task.FromResult(result);
        }
    }

    public class FakeTaskHistoryRepository : ITaskHistoryRepository
    {
        private readonly List<TaskHistory> _history = new();

        public FakeTaskHistoryRepository(IEnumerable<TaskHistory>? seed = null)
        {
            if (seed != null)
                _history.AddRange(seed);
        }

        public Task AddAsync(TaskHistory history)
        {
            _history.Add(history);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TaskHistory> historyRecords)
        {
            _history.AddRange(historyRecords);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId)
        {
            IEnumerable<TaskHistory> result =
                _history.Where(h => h.TaskItemId == taskId).ToList();
            return Task.FromResult(result);
        }
    }

    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public FakeUserRepository(IEnumerable<User>? seed = null)
        {
            if (seed != null)
                _users.AddRange(seed);
        }

        public Task<User> GetByIdAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId)
                       ?? new User(userId, "FakeUser", "fake@fake.com", "user");
            return Task.FromResult(user);
        }
    }
}

