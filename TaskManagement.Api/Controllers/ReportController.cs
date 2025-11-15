using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Handlers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly GetPerformanceReportQueryHandler _reportHandler;

    public ReportController(GetPerformanceReportQueryHandler reportHandler)
    {
        _reportHandler = reportHandler;
    }

    // Simulação do ID do usuário autenticado. ID 1 é 'Gerente 1' no seed.sql.
    private int GetCurrentUserId() => 1;

    /// <summary>
    /// Fornece o Relatório de Desempenho (acesso restrito a 'Gerente').
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPerformanceReport()
    {
        var query = new GetPerformanceReportQuery { UserId = GetCurrentUserId() };

        try
        {
            var report = await _reportHandler.Handle(query);
            return Ok(report);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Retorna 403 Forbidden se o usuário não for gerente
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (Exception)
        {
            // Logar o erro
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Erro interno ao gerar o relatório." });
        }
    }

    //[Authorize(Roles = "Manager")]
    //[HttpGet("performance")]
    //public async Task<IActionResult> GetPerformanceReport()
    //{
    //    var result = await _handler.Handle();
    //    return Ok(result);
    //}

}