using Domain.Entities;

namespace Domain.Repositories;

public interface ICaronaRepository
{
    Task<int> CreateAsync(Carona carona);
    Task<Carona?> GetByIdAsync(int id);
    Task<IEnumerable<Carona>> SearchAsync(string? origem, string? destino, DateTime? data);
    Task<IEnumerable<Carona>> GetByMotoristaIdAsync(int motoristaId);
    Task<bool> UpdateStatusAsync(int id, string status);
    Task<bool> DecrementVagaAsync(int id);
    Task<bool> IncrementVagaAsync(int id);
    Task<bool> DeleteAsync(int id);
}
