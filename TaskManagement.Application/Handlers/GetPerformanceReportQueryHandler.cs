using System;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Handlers
{
    public class PerformanceReportDto
    {
        public int CompletedTasksLast30Days { get; set; }
        public double AverageCompletionTimeDays { get; set; }
        public string ReportGeneratedBy { get; set; }
        public int AvgCompletionDays { get; set; }
        public int CompletedTasks { get; set; }
        public int LastDays { get; set; }
        public int TotalTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }

    }

    public class GetPerformanceReportQuery
    {
        public int UserId { get; set; } // Usuário solicitando
        public int LastDays { get; set; }
    }

    public class GetPerformanceReportQueryHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskItemRepository _taskRepository;

        //public GetPerformanceReportQueryHandler(IUserRepository userRepository, 
        //            ITaskItemRepository taskRepository
        //    )
        //{
        //    _userRepository = userRepository;
        //    _taskRepository = taskRepository;
        //}
        //private readonly IUserRepository _userRepository;
        //private readonly ITaskItemRepository _taskRepository;
        //private TaskManagement.Tests.Fakes.FakeTaskItemRepository taskRepo;

        //public GetPerformanceReportQueryHandler(TaskManagement.Tests.Fakes.FakeTaskItemRepository taskRepo)
        //{
        //    this.taskRepo = taskRepo;
        //}

        public GetPerformanceReportQueryHandler(
            IUserRepository userRepository,
            ITaskItemRepository taskRepository)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        public async Task<PerformanceReportDto> Handle(GetPerformanceReportQuery query)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId);

            if (user == null || !user.IsManager())
            {
                throw new UnauthorizedAccessException(
                    "Acesso negado. Apenas usuários com função 'gerente' podem visualizar relatórios.");
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var tasks = await _taskRepository.GetAllAsync();

            //Últimos 30 dias
            var cutoff = DateTime.UtcNow.AddDays(-30);
            var completedLast30 = tasks
                .Where(t => t.CompletedAt != null && t.CompletedAt >= cutoff)
                .Count();

            //Tempo médio de conclusão
            var avgCompletionDays = tasks
                //.Where(t => t.CompletedAt != null)
                //.Where(t => t.CompletedAt != null && t.CreatedAt != null) // <-- Adicione a checagem de CompletedAt e CreatedAt 
                .Where(t => t.CreatedAt.HasValue && t.CompletedAt.HasValue)
                .Average(t => (t.CompletedAt.Value - t.CreatedAt.Value).TotalDays);

            var completedTasks = tasks
                .Where(t => t.CreatedByUserId == query.UserId)
                .Where(t => t.Status == TaskManagement.Domain.Enums.TaskStatus.Completed)
                    //.Where(t => t.CompletedAt != null && t.CreatedAt != null) // <-- Adicione a checagem de CompletedAt e CreatedAt
                    .Where(t => t.CreatedAt.HasValue && t.CompletedAt.HasValue)
                    .Where(t => t.CompletedAt >= cutoffDate)
                .ToList();

            int completedLast30Days = completedTasks.Count;

            double averageCompletionTime = completedTasks.Any()
                     ? Math.Round(completedTasks
                         .Where(t => t.CreatedAt.HasValue && t.CompletedAt.HasValue)
                         .Average(t => (t.CompletedAt.Value - t.CreatedAt.Value).TotalDays), 2)
                     : 0.0;

            var totalTasks = tasks.Count;
            //var completed = allTasks.Count(t => t.Status == TaskManagement.Domain.Enums.TaskStatus.Completed);
            var inProgress = tasks.Count(t => t.Status == TaskManagement.Domain.Enums.TaskStatus.InProgress);
            var pending = tasks.Count(t => t.Status == TaskManagement.Domain.Enums.TaskStatus.Pending);

            return new PerformanceReportDto
            {
                CompletedTasksLast30Days = completedLast30Days,
                AverageCompletionTimeDays = Math.Round(averageCompletionTime, 2),
                ReportGeneratedBy = user.Name,
                CompletedTasks = completedLast30,
                AvgCompletionDays = (int)Math.Round(avgCompletionDays, 2),
                TotalTasks = totalTasks,
                InProgressTasks = inProgress,
                PendingTasks = pending
            };
        }
    }
}