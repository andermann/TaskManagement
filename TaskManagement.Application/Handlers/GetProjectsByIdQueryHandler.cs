using TaskManagement.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Application.Handlers
{
    public class GetProjectsByIdQueryHandler
    {
        private readonly IProjectRepository _repository;

        public GetProjectsByIdQueryHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        // 1. Listagem de Projetos - listar todos os projetos do usuário
        public async Task<List<ProjectListDto>> Handle(GetProjectsByIdQuery query)
        {
            var projects = await _repository.GetProjectsByIdAsync(query.Id);

            return projects.Select(p => new ProjectListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                // Assumindo que o repositório pode carregar a contagem ou que faremos uma query separada
                TaskCount = 0 // Implementação simplificada
            }).ToList();
        }


    }
}
