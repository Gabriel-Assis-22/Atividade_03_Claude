using Domain.Entities;

namespace Domain.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task UpdateProfileAsync(int id, string? bio);
    Task UpdateFotoAsync(int id, string fotoChave, string fotoUrl);
    Task SetPremiumAsync(int id, bool isPremium, string? stripeCustomerId);
    Task<(int TotalUsuarios, int TotalFree, int TotalPro, decimal MrrEstimado)> GetMetricasAssinaturasAsync();
}
