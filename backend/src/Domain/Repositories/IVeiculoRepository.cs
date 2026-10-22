using Domain.Entities;

namespace Domain.Repositories;

public interface IVeiculoRepository
{
    Task<int> CreateAsync(Veiculo veiculo);
    Task<Veiculo?> GetByIdAsync(int id);
    Task<IEnumerable<Veiculo>> GetByUsuarioIdAsync(int usuarioId);
}
