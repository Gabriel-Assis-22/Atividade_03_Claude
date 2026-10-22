using Application.DTOs.Veiculos;
using Application.UseCases.Veiculos;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/veiculos")]
public class VeiculosController(
    VeiculoUseCases veiculoUseCases,
    IStorageService storageService) : ControllerBase
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
    public async Task<IActionResult> CadastrarVeiculo([FromBody] CriarVeiculoDto dto)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        if (string.IsNullOrWhiteSpace(dto.Modelo) || string.IsNullOrWhiteSpace(dto.Placa))
            return BadRequest(new { erro = "Modelo e placa são obrigatórios." });

        try
        {
            var id = await veiculoUseCases.CriarVeiculoAsync(CurrentUserId.Value, dto);
            return StatusCode(StatusCodes.Status201Created, new { id, mensagem = "Veículo cadastrado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = ex.Message });
        }
    }

    [HttpGet("meus")]
    [Authorize]
    public async Task<IActionResult> ListarMeusVeiculos()
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        var veiculos = await veiculoUseCases.ListarMeusVeiculosAsync(CurrentUserId.Value);
        return Ok(veiculos);
    }

    [HttpPost("upload-foto")]
    [Authorize(Roles = "motorista,admin")]
    public async Task<IActionResult> UploadFoto([FromForm] IFormFile foto)
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Usuário não autenticado." });

        if (foto == null || foto.Length == 0)
            return BadRequest(new { erro = "Nenhum arquivo de imagem enviado." });

        var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();
        var extensoesValidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!extensoesValidas.Contains(extensao))
            return BadRequest(new { erro = "Formato de imagem inválido. Aceitos: JPG, PNG, WEBP." });

        var objectKey = $"veiculos/user_{CurrentUserId.Value}_{Guid.NewGuid():N}{extensao}";
        using var stream = foto.OpenReadStream();
        await storageService.UploadFileAsync(stream, objectKey, foto.ContentType);

        var publicUrl = storageService.GetPublicUrl(objectKey);
        return Ok(new { fotoUrl = publicUrl, objectKey });
    }
}
