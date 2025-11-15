namespace TaskManagement.Domain.Entities
{
    // A Entidade Project é um Aggregate Root
    public class Project
    {
        public const int MaxTasks = 20;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }

        // Regra de Negócio: Limite de Tarefas por Projeto
        // Esta lista é mantida apenas pelo Domain Service para verificar a regra.
        // A lista não é salva diretamente, mas é usada para verificar o estado do aggregate.
        private List<TaskItem> _tasks = new List<TaskItem>();

        public Project(string title, string description, int ownerId)
        {
            Title = title;
            Description = description;
            OwnerId = ownerId;
        }

        // Método para validar se é possível adicionar uma nova tarefa
        public void CheckTaskLimit()
        {
            if (_tasks.Count >= MaxTasks)
            {
                // Throwing an exception is a common DDD practice for business rule violations
                throw new InvalidOperationException($"O projeto '{Title}' atingiu o limite máximo de {MaxTasks} tarefas.");
            }
        }

        // Usado pelo Application Service para verificar a regra de remoção
        public bool HasPendingTasks()
        {
            // Pending (0) e In Progress (1)
            return _tasks.Any(t => t.Status == Enums.TaskStatus.Pending || t.Status == Enums.TaskStatus.InProgress); 
        }
    }
}