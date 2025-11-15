using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    // Query simples para listar tarefas de um projeto
    public class GetTasksByProjectIdQuery
    {
        public int ProjectId { get; set; }
    }

    public class GetTasksByProjectIdQueryHandler
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ITaskItemRepository _taskRepo;

        public GetTasksByProjectIdQueryHandler(
            IProjectRepository projectRepo,
            ITaskItemRepository taskRepo)
        {
            _projectRepo = projectRepo;
            _taskRepo = taskRepo;
        }

        public async Task<IEnumerable<TaskItem>> Handle(GetTasksByProjectIdQuery request)
        {
            // valida se o projeto existe (404 na controller se quiser)
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project is null)
                throw new Exception("Projeto não encontrado.");

            var tasks = await _taskRepo.GetByProjectIdAsync(request.ProjectId);
            return tasks;
        }
    }
}
