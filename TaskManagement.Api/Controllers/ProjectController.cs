
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Queries;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    // Handlers
    private readonly CreateProjectCommandHandler _createHandler;
    private readonly GetProjectsByOwnerIdQueryHandler _listHandler;
    private readonly GetProjectsAllQueryHandler _listAllHandler;
    private readonly GetProjectsByIdQueryHandler _listByIdHandler;
    private readonly DeleteProjectCommandHandler _deleteHandler;
    private readonly GetTasksByProjectIdQueryHandler _tasksByProjectHandler;


    public ProjectController(
        CreateProjectCommandHandler createHandler,
        GetProjectsByOwnerIdQueryHandler listHandler,
        GetProjectsAllQueryHandler listAllHandler,
        GetProjectsByIdQueryHandler listByIdHandler,
        DeleteProjectCommandHandler deleteHandler,
        GetTasksByProjectIdQueryHandler tasksByProjectHandler
        )
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _listAllHandler = listAllHandler;
        _listByIdHandler = listByIdHandler;
        _deleteHandler = deleteHandler;
        _tasksByProjectHandler = tasksByProjectHandler;
    }

    // Simulação do ID do usuário autenticado (User 1 - ID 1 no seed)
    private int GetCurrentUserId() => 1;

    /// <summary>
    /// 1. Listagem de Projetos - listar todos os projetos do usuário( User ID)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProjects()
    {
        var query = new GetProjectsByOwnerIdQuery { OwnerId = GetCurrentUserId() };
        var projects = await _listHandler.Handle(query);
        return Ok(projects);
    }

    /// <summary>
    /// 2. Listagem Geral de Projetos - retorna todos os projetos existentes
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllProjects()
    {
        var projects = await _listAllHandler.Handle();
        return Ok(projects);
    }

    /// <summary>
    /// 3. Lista todas as tarefas de um projeto pelo ID do projeto
    /// </summary>
    [HttpGet("{id:int}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProjectTasks(int id)
    {
        try
        {
            var tasks = await _tasksByProjectHandler.Handle(new GetTasksByProjectIdQuery
            {
                ProjectId = id
            });
            return Ok(tasks);
        }
        catch (Exception ex) when (ex.Message.Contains("não encontrado"))
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 4. Visualização de um Projeto (Faltou a implementação do Query Handler)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id)
    {
        var query = new GetProjectsByIdQuery { Id = id };
        var projects = await _listByIdHandler.Handle(query);
        return Ok(projects);

    }


    /// <summary>
    /// 5. Criação de Projetos - criar um novo projeto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        //command.OwnerId = GetCurrentUserId();

        await _createHandler.Handle(command);
        return StatusCode(StatusCodes.Status201Created);
    }

    

    /// <summary>
    /// 6. Remove um projeto pelo ID, se não houver tarefas pendentes
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var cmd = new DeleteProjectCommand
            {
                ProjectId = id
                //,UserId = GetCurrentUserId() // se já existir esse método
            };

            await _deleteHandler.Handle(cmd);
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

    

}