using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{

    // ----- COMMAND -----
    public class DeleteTaskCommand
    {
        public int ProjectId { get; set; }
        public int TaskId { get; set; }
    }

    public  class DeleteTaskCommandHandler
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ITaskItemRepository _taskRepo;

        public DeleteTaskCommandHandler(
            IProjectRepository projectRepo,
            ITaskItemRepository taskRepo)
        {
            _projectRepo = projectRepo;
            _taskRepo = taskRepo;
        }

        public async Task Handle(DeleteTaskCommand request)
        {
            // 1️ Buscar projeto
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new Exception("Projeto não encontrado.");

            // 2️ Verificar se há tarefas pendentes
            var hasPendingTasks = await _taskRepo.HasPendingTasksAsync(request.ProjectId);
            if (hasPendingTasks)
                throw new InvalidOperationException(
                    "Não é possível remover o projeto enquanto houver tarefas pendentes.");

            // 3️ Remover
            await _taskRepo.RemoveAsync(new TaskItem { Id = request.TaskId });
        }


    }
}
