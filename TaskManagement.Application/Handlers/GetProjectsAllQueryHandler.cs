// Application/Handlers/GetProjectsAllQueryHandler.cs
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    public class GetProjectsAllQueryHandler
    {
        private readonly IProjectRepository _repo;

        public GetProjectsAllQueryHandler(IProjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Project>> Handle()
        {
            return await _repo.GetAllAsync();
        }
    }
}
