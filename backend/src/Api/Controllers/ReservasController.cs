using Application.UseCases.Reservas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservasController(ReservaUseCases reservaUseCases) : ControllerBase
{
    private int? CurrentUserId
    {
        get
        {
            var rawId = User.FindFirst("userId")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? User.FindFirst("id")?.Value;

            if (int.TryParse(rawId, out var id)) return id;
            return null;
        }
    }

    [HttpGet("minhas")]
    [Authorize]
    public async Task<IActionResult> ListarMinhasReservas()
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        var reservas = await reservaUseCases.ListarMinhasReservasAsync(CurrentUserId.Value);
        return Ok(reservas);
    }

    [HttpPost("{id:int}/cancelar")]
    [Authorize]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        try
        {
            var cancelada = await reservaUseCases.CancelarReservaAsync(CurrentUserId.Value, id);
            if (!cancelada)
                return NotFound(new { erro = "Reserva não encontrada." });

            return Ok(new { mensagem = "Reserva cancelada com sucesso e vaga restaurada." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = ex.Message });
        }
    }
}
