//using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    // ----- COMMAND -----
    public class DeleteProjectCommand
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }  // caso queira logar quem removeu
    }

    // ----- HANDLER -----
    public class DeleteProjectCommandHandler
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ITaskItemRepository _taskRepo;

        public DeleteProjectCommandHandler(
            IProjectRepository projectRepo,
            ITaskItemRepository taskRepo)
        {
            _projectRepo = projectRepo;
            _taskRepo = taskRepo;
        }

        public async Task Handle(DeleteProjectCommand request)
        {
            // 1️ Buscar projeto
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null || project.Id == 0)
                throw new Exception("Projeto não encontrado.");

            // 2️ Verificar se há tarefas pendentes
            var hasPendingTasks = await _taskRepo.HasPendingTasksAsync(request.ProjectId);
            if (hasPendingTasks)
                throw new InvalidOperationException(
                    "Não é possível remover o projeto enquanto houver tarefas pendentes.");

            // 3️ Remover
            await _projectRepo.RemoveAsync(project.Id);
        }
    }
}
