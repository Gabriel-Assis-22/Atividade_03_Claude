using Dapper;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Persistence;

public class UsuarioRepository(DbConnectionFactory factory) : IUsuarioRepository
{
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        using var conn = factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Usuario>(
            @"SELECT id AS Id, 
                     nome AS Nome, 
                     email AS Email, 
                     role AS Role, 
                     foto_chave AS FotoChave, 
                     foto_url AS FotoUrl, 
                     bio AS Bio, 
                     is_premium AS IsPremium,
                     stripe_customer_id AS StripeCustomerId,
                     criado_em AS CriadoEm 
              FROM usuarios 
              WHERE id = @Id",
            new { Id = id });
    }

    public async Task UpdateProfileAsync(int id, string? bio)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE usuarios SET bio = @Bio WHERE id = @Id",
            new { Id = id, Bio = bio });
    }

    public async Task UpdateFotoAsync(int id, string fotoChave, string fotoUrl)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE usuarios SET foto_chave = @FotoChave, foto_url = @FotoUrl WHERE id = @Id",
            new { Id = id, FotoChave = fotoChave, FotoUrl = fotoUrl });
    }

    public async Task SetPremiumAsync(int id, bool isPremium, string? stripeCustomerId)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE usuarios SET is_premium = @IsPremium, stripe_customer_id = @StripeCustomerId WHERE id = @Id",
            new { Id = id, IsPremium = isPremium ? 1 : 0, StripeCustomerId = stripeCustomerId });
    }

    public async Task<(int TotalUsuarios, int TotalFree, int TotalPro, decimal MrrEstimado)> GetMetricasAssinaturasAsync()
    {
        using var conn = factory.CreateConnection();
        var sql = @"
            SELECT 
                COUNT(*) AS TotalUsuarios,
                SUM(CASE WHEN is_premium = 0 THEN 1 ELSE 0 END) AS TotalFree,
                SUM(CASE WHEN is_premium = 1 THEN 1 ELSE 0 END) AS TotalPro
            FROM usuarios";

        var result = await conn.QuerySingleOrDefaultAsync<dynamic>(sql);
        int total = result != null ? (int)result.TotalUsuarios : 0;
        int free = result != null ? Convert.ToInt32(result.TotalFree ?? 0) : 0;
        int pro = result != null ? Convert.ToInt32(result.TotalPro ?? 0) : 0;
        decimal mrr = pro * 14.90m;

        return (total, free, pro, mrr);
    }
}
