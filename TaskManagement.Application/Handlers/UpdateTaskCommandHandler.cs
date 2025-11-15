using System;
using System.Text.Json;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Handlers
{
    public class UpdateTaskCommand
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Domain.Enums.TaskStatus? Status { get; set; }
        //public string? Priority { get; set; }
        public TaskPriority? Priority { get; set; } = null;
        public int UserId { get; set; }
    }

    public class UpdateTaskCommandHandler
    {
        private readonly ITaskItemRepository _taskRepo;
        private readonly ITaskHistoryRepository _historyRepo;

        public UpdateTaskCommandHandler(ITaskItemRepository taskRepo, ITaskHistoryRepository historyRepo)
        {
            _taskRepo = taskRepo;
            _historyRepo = historyRepo;
        }

        public async Task Handle(UpdateTaskCommand request)
        {
            var task = await _taskRepo.GetByIdAsync(request.Id);
            if (task == null)
                throw new Exception("Tarefa não encontrada.");

            // Regra 1: impedir alteração de prioridade após criação
            if (request.Priority.HasValue)
                if (request.Priority != task.Priority)
                    throw new InvalidOperationException("A prioridade da tarefa não pode ser alterada após a criação.");

            // Atualiza dados permitidos
            if (!string.IsNullOrEmpty(request.Title))
                task.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                task.Description = request.Description;

            if (request.DueDate.HasValue)
                task.DueDate = request.DueDate.Value;

            if (request.Status.HasValue)
                task.Status = request.Status;

            // Atualiza data de modificação
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepo.UpdateAsync(task);

            // Registra histórico de atualização
            var history = new TaskHistory
            {
                TaskItemId = task.Id,
                ModifiedAt = DateTime.UtcNow,
                ModifiedByUserId = request.UserId,
                Field = JsonSerializer.Serialize(new
                {
                    //request.Title,
                    request.Description,
                    request.DueDate,
                    request.Status
                })

            };
            await _historyRepo.AddAsync(history);
        }
    }
}
