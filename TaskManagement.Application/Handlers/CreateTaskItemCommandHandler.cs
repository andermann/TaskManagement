using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Handlers
{
    //public class CreateTaskItemCommand
    //{
    //    public int ProjectId { get; set; }
    //    public string Title { get; set; } = string.Empty;
    //    public string? Description { get; set; }
    //    //public string Priority { get; set; } = "Medium"; // default
    //    public TaskPriority Priority { get; set; } // 0: Low, 1: Medium, 2: High
    //    public DateTime? DueDate { get; set; }
    //    public int UserId { get; set; }
    //}




    public class CreateTaskItemCommandHandler
    {
        private readonly ITaskItemRepository _taskRepo;
        private readonly IProjectRepository _projectRepo;

        public CreateTaskItemCommandHandler(
            ITaskItemRepository taskRepo,
            IProjectRepository projectRepo)
        {
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
        }

        public async Task Handle(CreateTaskItemCommand request)
        {
            //  Verifica se o projeto existe
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new Exception("Projeto não encontrado.");

            // 2️ Verifica limite de 20 tarefas por projeto
            var count = await _taskRepo.CountByProjectAsync(request.ProjectId);
            if (count >= 20)
                throw new InvalidOperationException("Cada projeto pode ter no máximo 20 tarefas.");

            // 3️ Cria a tarefa
            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Priority = (Domain.Enums.TaskPriority)request.Priority,
                DueDate = request.DueDate,
                Status = Domain.Enums.TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.UserId
            };

            await _taskRepo.AddAsync(task);
        }
    }
}
