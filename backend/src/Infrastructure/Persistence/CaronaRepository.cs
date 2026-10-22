using Dapper;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Persistence;

public class CaronaRepository(DbConnectionFactory factory) : ICaronaRepository
{
    public async Task<int> CreateAsync(Carona carona)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO caronas (motorista_id, veiculo_id, cidade_origem, cidade_destino, ponto_encontro, 
                                 data_hora_saida, vagas_totais, vagas_disponiveis, valor_rateio, status, created_at)
            VALUES (@MotoristaId, @VeiculoId, @CidadeOrigem, @CidadeDestino, @PontoEncontro, 
                    @DataHoraSaida, @VagasTotais, @VagasDisponiveis, @ValorRateio, @Status, NOW());
            SELECT LAST_INSERT_ID();
        ", carona);
    }

    public async Task<Carona?> GetByIdAsync(int id)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Carona>(@"
            SELECT c.id AS Id, c.motorista_id AS MotoristaId, c.veiculo_id AS VeiculoId,
                   c.cidade_origem AS CidadeOrigem, c.cidade_destino AS CidadeDestino,
                   c.ponto_encontro AS PontoEncontro, c.data_hora_saida AS DataHoraSaida,
                   c.vagas_totais AS VagasTotais, c.vagas_disponiveis AS VagasDisponiveis,
                   c.valor_rateio AS ValorRateio, c.status AS Status, c.created_at AS CreatedAt,
                   u.nome AS MotoristaNome, u.foto_url AS MotoristaFotoUrl,
                   v.modelo AS VeiculoModelo, v.placa AS VeiculoPlaca
            FROM caronas c
            INNER JOIN usuarios u ON u.id = c.motorista_id
            INNER JOIN veiculos v ON v.id = c.veiculo_id
            WHERE c.id = @Id
        ", new { Id = id });
    }

    public async Task<IEnumerable<Carona>> SearchAsync(string? origem, string? destino, DateTime? data)
    {
        using var conn = factory.CreateConnection();
        var sql = @"
            SELECT c.id AS Id, c.motorista_id AS MotoristaId, c.veiculo_id AS VeiculoId,
                   c.cidade_origem AS CidadeOrigem, c.cidade_destino AS CidadeDestino,
                   c.ponto_encontro AS PontoEncontro, c.data_hora_saida AS DataHoraSaida,
                   c.vagas_totais AS VagasTotais, c.vagas_disponiveis AS VagasDisponiveis,
                   c.valor_rateio AS ValorRateio, c.status AS Status, c.created_at AS CreatedAt,
                   u.nome AS MotoristaNome, u.foto_url AS MotoristaFotoUrl,
                   v.modelo AS VeiculoModelo, v.placa AS VeiculoPlaca
            FROM caronas c
            INNER JOIN usuarios u ON u.id = c.motorista_id
            INNER JOIN veiculos v ON v.id = c.veiculo_id
            WHERE c.status = 'disponivel' AND c.vagas_disponiveis > 0
        ";

        var builder = new DynamicParameters();
        if (!string.IsNullOrWhiteSpace(origem))
        {
            sql += " AND c.cidade_origem LIKE @Origem";
            builder.Add("Origem", $"%{origem}%");
        }

        if (!string.IsNullOrWhiteSpace(destino))
        {
            sql += " AND c.cidade_destino LIKE @Destino";
            builder.Add("Destino", $"%{destino}%");
        }

        if (data.HasValue)
        {
            sql += " AND DATE(c.data_hora_saida) = DATE(@Data)";
            builder.Add("Data", data.Value);
        }

        sql += " ORDER BY c.data_hora_saida ASC";

        return await conn.QueryAsync<Carona>(sql, builder);
    }

    public async Task<IEnumerable<Carona>> GetByMotoristaIdAsync(int motoristaId)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryAsync<Carona>(@"
            SELECT c.id AS Id, c.motorista_id AS MotoristaId, c.veiculo_id AS VeiculoId,
                   c.cidade_origem AS CidadeOrigem, c.cidade_destino AS CidadeDestino,
                   c.ponto_encontro AS PontoEncontro, c.data_hora_saida AS DataHoraSaida,
                   c.vagas_totais AS VagasTotais, c.vagas_disponiveis AS VagasDisponiveis,
                   c.valor_rateio AS ValorRateio, c.status AS Status, c.created_at AS CreatedAt,
                   u.nome AS MotoristaNome, u.foto_url AS MotoristaFotoUrl,
                   v.modelo AS VeiculoModelo, v.placa AS VeiculoPlaca
            FROM caronas c
            INNER JOIN usuarios u ON u.id = c.motorista_id
            INNER JOIN veiculos v ON v.id = c.veiculo_id
            WHERE c.motorista_id = @MotoristaId
            ORDER BY c.data_hora_saida DESC
        ", new { MotoristaId = motoristaId });
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync(@"
            UPDATE caronas SET status = @Status WHERE id = @Id
        ", new { Id = id, Status = status });
        return rows > 0;
    }

    public async Task<bool> DecrementVagaAsync(int id)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync(@"
            UPDATE caronas 
            SET vagas_disponiveis = vagas_disponiveis - 1,
                status = CASE WHEN vagas_disponiveis - 1 <= 0 THEN 'lotada' ELSE status END
            WHERE id = @Id AND vagas_disponiveis > 0
        ", new { Id = id });
        return rows > 0;
    }

    public async Task<bool> IncrementVagaAsync(int id)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync(@"
            UPDATE caronas 
            SET vagas_disponiveis = vagas_disponiveis + 1,
                status = 'disponivel'
            WHERE id = @Id AND vagas_disponiveis < vagas_totais
        ", new { Id = id });
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync("DELETE FROM caronas WHERE id = @Id", new { Id = id });
        return rows > 0;
    }
}
