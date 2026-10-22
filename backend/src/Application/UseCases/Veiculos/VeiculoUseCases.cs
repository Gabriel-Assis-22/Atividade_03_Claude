using Application.DTOs.Veiculos;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.ExternalServices;

namespace Application.UseCases.Veiculos;

public class VeiculoUseCases(
    IVeiculoRepository veiculoRepo,
    IAuditClient auditClient)
{
    public async Task<int> CriarVeiculoAsync(int motoristaId, CriarVeiculoDto dto)
    {
        var veiculo = new Veiculo
        {
            UsuarioId = motoristaId,
            Modelo = dto.Modelo,
            Placa = dto.Placa.ToUpper().Trim(),
            Cor = dto.Cor,
            Ano = dto.Ano,
            FotoUrl = dto.FotoUrl
        };

        var id = await veiculoRepo.CreateAsync(veiculo);

        await auditClient.LogAsync(
            motoristaId, 
            "veiculo_cadastrado", 
            $"Veículo {dto.Modelo} ({dto.Placa}) cadastrado com ID {id}.");

        return id;
    }

    public async Task<IEnumerable<Veiculo>> ListarMeusVeiculosAsync(int motoristaId)
    {
        return await veiculoRepo.GetByUsuarioIdAsync(motoristaId);
    }
}
