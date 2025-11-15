using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Queries;
using System.Threading.Tasks;

namespace TaskManagement.Application.Handlers
{
    public class GetTaskItemByIdQueryHandler
    {
        private readonly ITaskItemRepository _repository;

        public GetTaskItemByIdQueryHandler(ITaskItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskItemDto> Handle(GetTaskItemByIdQuery query)
        {
            var task = await _repository.GetByIdAsync(query.TaskId);

            if (task == null)
            {
                return null; // Retorna nulo, o Controller cuida do 404
            }

            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                //Status = task.Status.ToString(),
                Status = (Domain.Enums.TaskStatus)task.Status,
                //Priority = task.Priority.ToString(),
                Priority = (Domain.Enums.TaskPriority)task.Priority,
                ProjectId = task.ProjectId,
                UserId = task.CreatedByUserId
            };
        }
    }
}