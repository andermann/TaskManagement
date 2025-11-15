namespace TaskManagement.Application.Queries
{
    public class GetPerformanceReportResult
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }

        public decimal CompletionRate =>
            TotalTasks == 0 ? 0 : (decimal)CompletedTasks / TotalTasks * 100;
    }
}
