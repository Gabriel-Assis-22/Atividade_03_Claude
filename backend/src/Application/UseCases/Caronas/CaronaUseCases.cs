using Application.DTOs.Caronas;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.ExternalServices;

namespace Application.UseCases.Caronas;

public class CaronaUseCases(
    ICaronaRepository caronaRepo,
    IVeiculoRepository veiculoRepo,
    IAuditClient auditClient)
{
    public async Task<int> CriarCaronaAsync(int motoristaId, CriarCaronaDto dto)
    {
        // Validação de posse do veículo (anti-IDOR)
        var veiculo = await veiculoRepo.GetByIdAsync(dto.VeiculoId);
        if (veiculo is null || veiculo.UsuarioId != motoristaId)
        {
            await auditClient.LogAsync(
                motoristaId, 
                "bloqueio_seguranca", 
                $"Tentativa não autorizada de criar carona com veículo {dto.VeiculoId} que não pertence ao usuário.");
            throw new UnauthorizedAccessException("O veículo selecionado não pertence ao motorista.");
        }

        var carona = new Carona
        {
            MotoristaId = motoristaId,
            VeiculoId = dto.VeiculoId,
            CidadeOrigem = dto.CidadeOrigem.Trim(),
            CidadeDestino = dto.CidadeDestino.Trim(),
            PontoEncontro = dto.PontoEncontro.Trim(),
            DataHoraSaida = dto.DataHoraSaida,
            VagasTotais = dto.VagasTotais,
            VagasDisponiveis = dto.VagasTotais,
            ValorRateio = dto.ValorRateio,
            Status = "disponivel"
        };

        var id = await caronaRepo.CreateAsync(carona);

        await auditClient.LogAsync(
            motoristaId, 
            "carona_criada", 
            $"Carona #{id} criada de {carona.CidadeOrigem} para {carona.CidadeDestino} com {carona.VagasTotais} vagas.");

        return id;
    }

    public async Task<IEnumerable<Carona>> BuscarCaronasAsync(BuscarCaronaQuery query)
    {
        return await caronaRepo.SearchAsync(query.Origem, query.Destino, query.Data);
    }

    public async Task<Carona?> ObterPorIdAsync(int id)
    {
        return await caronaRepo.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Carona>> ListarMinhasCaronasAsync(int motoristaId)
    {
        return await caronaRepo.GetByMotoristaIdAsync(motoristaId);
    }

    public async Task<bool> CancelarCaronaAsync(int motoristaId, int caronaId)
    {
        var carona = await caronaRepo.GetByIdAsync(caronaId);
        if (carona is null) return false;

        // Proteção anti-IDOR: apenas o motorista dono da carona pode cancelar
        if (carona.MotoristaId != motoristaId)
        {
            await auditClient.LogAsync(
                motoristaId, 
                "bloqueio_seguranca", 
                $"Usuário {motoristaId} tentou cancelar indevidamente a carona #{caronaId} pertencente ao motorista {carona.MotoristaId}.");
            throw new UnauthorizedAccessException("Você não tem permissão para cancelar esta carona.");
        }

        var cancelada = await caronaRepo.UpdateStatusAsync(caronaId, "cancelada");
        if (cancelada)
        {
            await auditClient.LogAsync(
                motoristaId, 
                "carona_cancelada", 
                $"Carona #{caronaId} cancelada pelo motorista titular.");
        }

        return cancelada;
    }
}
