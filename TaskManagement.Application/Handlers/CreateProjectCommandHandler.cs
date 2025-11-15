using TaskManagement.Application.Commands;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    // O Handler executa a lógica do caso de uso
    public class CreateProjectCommandHandler
    {
        private readonly IProjectRepository _repository;
        // Injetar outros serviços de domínio ou notificação, se necessário

        public CreateProjectCommandHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateProjectCommand command)
        {
            // 1. Instanciar a entidade do domínio
            var project = new Project(command.Title, command.Description, command.OwnerId);

            // 2. Persistir no banco
            await _repository.AddAsync(project);
        }
    }
}