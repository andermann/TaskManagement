using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Queries;


[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly CreateTaskItemCommandHandler _createHandler;
    private readonly GetTaskItemByIdQueryHandler _getByIdHandler;
    private readonly UpdateTaskCommandHandler _updateHandler;
    private readonly AddTaskCommentCommandHandler _addCommentHandler;
    private readonly GetTasksCommentsByTaskIdQueryHandler _getCommentsHandler;

    private readonly GetTasksCommentsAndHistoryByTaskIdQueryHandler _getCommentsAndHistoryHandler;
    private readonly DeleteTaskCommandHandler _deleteTaskHandler;

    public TaskController(
        CreateTaskItemCommandHandler createHandler,
        GetTaskItemByIdQueryHandler getByIdHandler,
        UpdateTaskCommandHandler updateHandler,
        AddTaskCommentCommandHandler addCommentHandler,
        GetTasksCommentsByTaskIdQueryHandler getCommentsHandler,
        GetTasksCommentsAndHistoryByTaskIdQueryHandler getCommentsAndHistoryHandler,
        DeleteTaskCommandHandler deleteTaskHandler
        )
    {
        _createHandler = createHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler; 
        _addCommentHandler = addCommentHandler;
        _getCommentsHandler = getCommentsHandler;
        _getCommentsAndHistoryHandler = getCommentsAndHistoryHandler;
        _deleteTaskHandler = deleteTaskHandler;
        
    }

    // Simulação do ID do usuário autenticado (User 1 - ID 2 no seed.sql)
    private int GetCurrentUserId() => 1;

    /// <summary>
    /// 3. Criação de Tarefas - adicionar uma nova tarefa a um projeto
    /// (Envolve a regra de limite de 20 tarefas por projeto)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskItemCommand body)
    {
        try
        {
            var cmd = new CreateTaskItemCommand
            {
                ProjectId = body.ProjectId,
                Title = body.Title,
                Description = body.Description,
                Priority = body.Priority,
                DueDate = body.DueDate,
                UserId = body.UserId // GetCurrentUserId() // o que você já usa
            };


            await _createHandler.Handle(cmd);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (InvalidOperationException ex)
        {
            // Retorna 400 Bad Request se o limite de tarefas for atingido
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 2. Visualização de Tarefas - visualizar uma tarefa específica
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTask(int id)
    {
        var query = new GetTaskItemByIdQuery { TaskId = id };
        var taskDto = await _getByIdHandler.Handle(query);

        if (taskDto == null)
        {
            return NotFound($"Tarefa com ID {id} não encontrada.");
        }

        return Ok(taskDto);
    }

    /// <summary>Lista todos os comentários de uma tarefa</summary>
    [HttpGet("{id:int}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskComments(int id)
    {
        var query = new GetTasksCommentsByTaskIdQuery { TaskItemId = id };
        var comments = await _getCommentsHandler.Handle(query);

        if (comments == null || !comments.Any())
            return NotFound(new { message = "Nenhum comentário encontrado para esta tarefa." });

        return Ok(comments);
    }

    /// <summary>Comentários, histórico e timeline unificada da tarefa</summary>
    [HttpGet("{id:int}/comments-history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskCommentsAndHistory(int id)
    {
        var dto = await _getCommentsAndHistoryHandler.Handle(
            new GetTasksCommentsAndHistoryByTaskIdQuery { TaskItemId = id });

        // se quiser, retorne 404 quando não houver nem comentários nem history
        // if (!dto.Comments.Any() && !dto.History.Any()) return NotFound();

        return Ok(dto);
    }




    /// <summary>
    /// 4. Atualização de Tarefas - atualizar o status ou detalhes de uma tarefa
    /// (Este endpoint DEVE gerar um registro em TaskHistory)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskManagement.Application.Handlers.UpdateTaskCommand body)
    {

        var cmd = new TaskManagement.Application.Handlers.UpdateTaskCommand
        {
            Id = id,
            Title = body.Title,
            Description = body.Description,
            DueDate = body.DueDate,
            Status = body.Status,
            Priority = body.Priority,
            UserId = GetCurrentUserId() // Usuário que realiza a alteração
        };

        if (id != cmd.Id) return BadRequest("O ID da rota não corresponde ao ID do corpo da requisição.");

        try
        {
            await _updateHandler.Handle(cmd);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("Tarefa não encontrada")) return NotFound(ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 5. Remoção de Tarefas - remover uma tarefa de um projeto
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTask(int id, int ProjectId)
    {
        // Implementação do RemoveTaskItemCommandHandler
        //return Task.FromResult<IActionResult>(NoContent());

        try
        {
            var cmd = new DeleteTaskCommand
            {
                TaskId = id,
                ProjectId = ProjectId

            };

            await _deleteTaskHandler.Handle(cmd);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }

    }

    /// <summary>Adiciona um comentário à tarefa</summary>
    [HttpPost("{id:int}/comments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddTaskCommentRequest body)
    {
        if (string.IsNullOrWhiteSpace(body.Message))
            return BadRequest(new { message = "Conteúdo do comentário é obrigatório." });

        var cmd = new AddTaskCommentCommand
        {
            TaskItemId = id,
            Message = body.Message,
            UserId = 1
        };

        await _addCommentHandler.Handle(cmd);
        return NoContent();
    }

}