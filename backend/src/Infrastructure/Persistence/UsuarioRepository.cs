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
}
