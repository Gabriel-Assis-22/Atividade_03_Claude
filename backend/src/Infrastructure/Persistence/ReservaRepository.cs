using Dapper;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Persistence;

public class ReservaRepository(DbConnectionFactory factory) : IReservaRepository
{
    public async Task<int> CreateAsync(Reserva reserva)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO reservas (carona_id, passageiro_id, status, created_at)
            VALUES (@CaronaId, @PassageiroId, @Status, NOW());
            SELECT LAST_INSERT_ID();
        ", reserva);
    }

    public async Task<Reserva?> GetByIdAsync(int id)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Reserva>(@"
            SELECT r.id AS Id, r.carona_id AS CaronaId, r.passageiro_id AS PassageiroId, 
                   r.status AS Status, r.created_at AS CreatedAt,
                   u.nome AS PassageiroNome,
                   c.cidade_origem AS CidadeOrigem, c.cidade_destino AS CidadeDestino,
                   c.data_hora_saida AS DataHoraSaida, c.valor_rateio AS ValorRateio
            FROM reservas r
            INNER JOIN usuarios u ON u.id = r.passageiro_id
            INNER JOIN caronas c ON c.id = r.carona_id
            WHERE r.id = @Id
        ", new { Id = id });
    }

    public async Task<Reserva?> GetByCaronaEPassageiroAsync(int caronaId, int passageiroId)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Reserva>(@"
            SELECT id AS Id, carona_id AS CaronaId, passageiro_id AS PassageiroId, 
                   status AS Status, created_at AS CreatedAt
            FROM reservas
            WHERE carona_id = @CaronaId AND passageiro_id = @PassageiroId
        ", new { CaronaId = caronaId, PassageiroId = passageiroId });
    }

    public async Task<IEnumerable<Reserva>> GetByPassageiroIdAsync(int passageiroId)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryAsync<Reserva>(@"
            SELECT r.id AS Id, r.carona_id AS CaronaId, r.passageiro_id AS PassageiroId, 
                   r.status AS Status, r.created_at AS CreatedAt,
                   u.nome AS PassageiroNome,
                   c.cidade_origem AS CidadeOrigem, c.cidade_destino AS CidadeDestino,
                   c.data_hora_saida AS DataHoraSaida, c.valor_rateio AS ValorRateio
            FROM reservas r
            INNER JOIN usuarios u ON u.id = r.passageiro_id
            INNER JOIN caronas c ON c.id = r.carona_id
            WHERE r.passageiro_id = @PassageiroId
            ORDER BY r.created_at DESC
        ", new { PassageiroId = passageiroId });
    }

    public async Task<IEnumerable<Reserva>> GetByCaronaIdAsync(int caronaId)
    {
        using var conn = factory.CreateConnection();
        return await conn.QueryAsync<Reserva>(@"
            SELECT r.id AS Id, r.carona_id AS CaronaId, r.passageiro_id AS PassageiroId, 
                   r.status AS Status, r.created_at AS CreatedAt,
                   u.nome AS PassageiroNome
            FROM reservas r
            INNER JOIN usuarios u ON u.id = r.passageiro_id
            WHERE r.carona_id = @CaronaId
            ORDER BY r.created_at ASC
        ", new { CaronaId = caronaId });
    }

    public async Task<int> CountActiveByPassageiroIdAsync(int passageiroId)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(@"
            SELECT COUNT(1)
            FROM reservas r
            INNER JOIN caronas c ON c.id = r.carona_id
            WHERE r.passageiro_id = @PassageiroId
              AND r.status = 'confirmada'
              AND c.data_hora_saida >= NOW()
        ", new { PassageiroId = passageiroId });
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync(@"
            UPDATE reservas SET status = @Status WHERE id = @Id
        ", new { Id = id, Status = status });
        return rows > 0;
    }
}
