using Domain.Entities;

namespace Domain.Repositories;

public interface IReservaRepository
{
    Task<int> CreateAsync(Reserva reserva);
    Task<Reserva?> GetByIdAsync(int id);
    Task<Reserva?> GetByCaronaEPassageiroAsync(int caronaId, int passageiroId);
    Task<IEnumerable<Reserva>> GetByPassageiroIdAsync(int passageiroId);
    Task<IEnumerable<Reserva>> GetByCaronaIdAsync(int caronaId);
    Task<int> CountActiveByPassageiroIdAsync(int passageiroId);
    Task<bool> UpdateStatusAsync(int id, string status);
}
