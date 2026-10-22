using Dapper;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Persistence;

public class VeiculoRepository(DbConnectionFactory factory) : IVeiculoRepository
{
    public async Task<int> CreateAsync(Veiculo veiculo)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO veiculos (usuario_id, modelo, placa, cor, ano, foto_url, created_at)
            VALUES (@UsuarioId, @Modelo, @Placa, @Cor, @Ano, @FotoUrl, NOW());
            SELECT LAST_INSERT_ID();
        ", veiculo);
    }

    public async Task<Veiculo?> GetByIdAsync(int id)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Veiculo>(@"
            SELECT id AS Id, usuario_id AS UsuarioId, modelo AS Modelo, placa AS Placa, 
                   cor AS Cor, ano AS Ano, foto_url AS FotoUrl, created_at AS CreatedAt
            FROM veiculos 
            WHERE id = @Id
        ", new { Id = id });
    }

    public async Task<IEnumerable<Veiculo>> GetByUsuarioIdAsync(int usuarioId)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryAsync<Veiculo>(@"
            SELECT id AS Id, usuario_id AS UsuarioId, modelo AS Modelo, placa AS Placa, 
                   cor AS Cor, ano AS Ano, foto_url AS FotoUrl, created_at AS CreatedAt
            FROM veiculos 
            WHERE usuario_id = @UsuarioId
            ORDER BY id DESC
        ", new { UsuarioId = usuarioId });
    }
}
