using Application.DTOs.Caronas;
using Application.UseCases.Caronas;
using Application.UseCases.Reservas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/caronas")]
public class CaronasController(
    CaronaUseCases caronaUseCases,
    ReservaUseCases reservaUseCases) : ControllerBase
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

    [HttpPost]
    [Authorize(Roles = "motorista,admin")]
    public async Task<IActionResult> PublicarCarona([FromBody] CriarCaronaDto dto)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        if (string.IsNullOrWhiteSpace(dto.CidadeOrigem) || string.IsNullOrWhiteSpace(dto.CidadeDestino))
            return BadRequest(new { erro = "Origem e destino são obrigatórios." });

        if (dto.VagasTotais <= 0)
            return BadRequest(new { erro = "A carona deve conter ao menos 1 vaga." });

        try
        {
            var id = await caronaUseCases.CriarCaronaAsync(CurrentUserId.Value, dto);
            return StatusCode(StatusCodes.Status201Created, new { id, mensagem = "Carona publicada com sucesso." });
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

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListarCaronas([FromQuery] string? origem, [FromQuery] string? destino, [FromQuery] DateTime? data)
    {
        var query = new BuscarCaronaQuery(origem, destino, data);
        var caronas = await caronaUseCases.BuscarCaronasAsync(query);
        return Ok(caronas);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var carona = await caronaUseCases.ObterPorIdAsync(id);
        if (carona is null)
            return NotFound(new { erro = "Carona não encontrada." });

        return Ok(carona);
    }

    [HttpGet("minhas")]
    [Authorize]
    public async Task<IActionResult> ListarMinhasCaronas()
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        var caronas = await caronaUseCases.ListarMinhasCaronasAsync(CurrentUserId.Value);
        return Ok(caronas);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> CancelarCarona(int id)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        try
        {
            var cancelada = await caronaUseCases.CancelarCaronaAsync(CurrentUserId.Value, id);
            if (!cancelada)
                return NotFound(new { erro = "Carona não encontrada." });

            return Ok(new { mensagem = "Carona cancelada com sucesso." });
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

    [HttpPost("{id:int}/reservar")]
    [Authorize]
    public async Task<IActionResult> ReservarAssento(int id)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        try
        {
            var reservaId = await reservaUseCases.ReservarAssentoAsync(CurrentUserId.Value, id);
            return StatusCode(StatusCodes.Status201Created, new { reservaId, mensagem = "Reserva confirmada com sucesso!" });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("LIMITE_PLANO_FREE"))
        {
            // Enforcement do plano Free (HTTP 403 Forbidden com instrução de upgrade)
            return StatusCode(StatusCodes.Status403Forbidden, new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = ex.Message });
        }
    }
}
