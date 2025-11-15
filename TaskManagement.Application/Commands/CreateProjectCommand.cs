
namespace TaskManagement.Application.Commands // << Este é o namespace correto
{
    // DTO de entrada para o caso de uso
    public class CreateProjectCommand
    {
        public string? Title { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; } // O Id do usuário logado
    }
}