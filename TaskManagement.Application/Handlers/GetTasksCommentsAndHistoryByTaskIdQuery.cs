using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    // ==== DTOs de saída ====
    public record TaskCommentDto(
        int Id, 
        int TaskItemId, 
        string Message, 
        int? CreatedByUserId, 
        DateTime CreatedAt
        );

    public record TaskHistoryDto(
        int Id, int TaskItemId, string Field, 
        int? ModifiedByUserId, DateTime ModifiedAt);

    public record TimelineItemDto(
        string Type,           // "comment" | "history"
        DateTime When,
        int? UserId,
        string Title,          // Ex.: "Comentário" ou nome do campo alterado
        string Body            // Conteúdo do comentário ou descrição da mudança
    );

    public record TaskCommentsAndHistoryDto(
        int TaskItemId,
        IReadOnlyList<TaskCommentDto> Comments,
        IReadOnlyList<TaskHistoryDto> History,
        IReadOnlyList<TimelineItemDto> Timeline);

    // ==== Query ====
    public class GetTasksCommentsAndHistoryByTaskIdQuery
    {
        public int TaskItemId { get; set; }
    }

    // ==== Handler ====
    public class GetTasksCommentsAndHistoryByTaskIdQueryHandler
    {
        private readonly ITaskCommentRepository _commentRepo;
        private readonly ITaskHistoryRepository _historyRepo;

        public GetTasksCommentsAndHistoryByTaskIdQueryHandler(
            ITaskCommentRepository commentRepo,
            ITaskHistoryRepository historyRepo)
        {
            _commentRepo = commentRepo;
            _historyRepo = historyRepo;
        }

        public async Task<TaskCommentsAndHistoryDto> Handle(GetTasksCommentsAndHistoryByTaskIdQuery request)
        {
            var comments = await _commentRepo.GetByTaskIdAsync(request.TaskItemId);
            var history = await _historyRepo.GetByTaskIdAsync(request.TaskItemId);

            var commentDtos = comments
                .Select(c => new TaskCommentDto(
                    c.Id, c.TaskItemId, c.Message, c.CreatedByUserId, c.CreatedAt))
                .ToList();

            var historyDtos = history
                .Select(h => new TaskHistoryDto(
                    h.Id, h.TaskItemId, h.Field, h.ModifiedByUserId, h.ModifiedAt))
                .ToList();

            // timeline unificada (comentários + histórico)
            var timeline = new List<TimelineItemDto>();

            timeline.AddRange(commentDtos.Select(c =>
                new TimelineItemDto(
                    "comment",
                    c.CreatedAt,
                    c.CreatedByUserId,
                    "Comentário",
                    c.Message)));

            timeline.AddRange(historyDtos.Select(h =>
                new TimelineItemDto(
                    "history",
                    h.ModifiedAt,
                    h.ModifiedByUserId,
                    string.IsNullOrWhiteSpace(h.Field) ? "Alteração" : h.Field,  "}'"
                )));

            // ordem decrescente por data
            var orderedTimeline = timeline
                .OrderByDescending(t => t.When)
                .ToList();

            return new TaskCommentsAndHistoryDto(
                request.TaskItemId,
                commentDtos,
                historyDtos,
                orderedTimeline);
        }
    }
}
