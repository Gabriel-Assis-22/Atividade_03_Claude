using Domain.Entities;
using Domain.Repositories;
using Infrastructure.ExternalServices;

namespace Application.UseCases.Reservas;

public class ReservaUseCases(
    IReservaRepository reservaRepo,
    ICaronaRepository caronaRepo,
    IUsuarioRepository usuarioRepo,
    IAuditClient auditClient)
{
    public async Task<int> ReservarAssentoAsync(int passageiroId, int caronaId)
    {
        var carona = await caronaRepo.GetByIdAsync(caronaId);
        if (carona is null)
            throw new ArgumentException("Carona não encontrada.");

        // 1. Motorista não pode reservar a própria carona
        if (carona.MotoristaId == passageiroId)
            throw new InvalidOperationException("Motorista não pode reservar vaga na própria carona.");

        if (carona.Status != "disponivel" || carona.VagasDisponiveis <= 0)
            throw new InvalidOperationException("Não há vagas disponíveis nesta carona.");

        // 2. Não permitir reserva duplicada
        var existente = await reservaRepo.GetByCaronaEPassageiroAsync(caronaId, passageiroId);
        if (existente is not null && existente.Status == "confirmada")
            throw new InvalidOperationException("Você já possui uma reserva confirmada para esta carona.");

        // 3. Enforcement de Plano SaaS: Usuários sem plano Pro têm limite de no máximo 2 reservas ativas simultâneas
        var usuario = await usuarioRepo.GetByIdAsync(passageiroId);
        var isPro = usuario?.IsPremium ?? false;

        if (!isPro)
        {
            var reservasAtivas = await reservaRepo.CountActiveByPassageiroIdAsync(passageiroId);
            if (reservasAtivas >= 2)
            {
                await auditClient.LogAsync(
                    passageiroId,
                    "bloqueio_limite_plano",
                    $"Usuário do plano Free atingiu o limite de {reservasAtivas} reservas ativas simultâneas.");
                throw new InvalidOperationException("LIMITE_PLANO_FREE: Usuários do plano gratuito podem ter no máximo 2 reservas ativas simultâneas. Assine o UniRide Pro para reservas ilimitadas.");
            }
        }

        // 4. Decremento atômico de vaga no banco
        var decrementado = await caronaRepo.DecrementVagaAsync(caronaId);
        if (!decrementado)
            throw new InvalidOperationException("Vaga indisponível no momento.");

        // 5. Salva a reserva
        var reserva = new Reserva
        {
            CaronaId = caronaId,
            PassageiroId = passageiroId,
            Status = "confirmada"
        };

        var id = await reservaRepo.CreateAsync(reserva);

        await auditClient.LogAsync(
            passageiroId,
            "reserva_solicitada",
            $"Reserva #{id} confirmada para a carona #{caronaId} pelo passageiro {passageiroId}.");

        return id;
    }

    public async Task<bool> CancelarReservaAsync(int solicitanteId, int reservaId)
    {
        var reserva = await reservaRepo.GetByIdAsync(reservaId);
        if (reserva is null) return false;

        var carona = await caronaRepo.GetByIdAsync(reserva.CaronaId);

        // Proteção anti-IDOR: apenas o passageiro titular ou o motorista da carona pode cancelar
        var isPassageiro = (reserva.PassageiroId == solicitanteId);
        var isMotorista = (carona is not null && carona.MotoristaId == solicitanteId);

        if (!isPassageiro && !isMotorista)
        {
            await auditClient.LogAsync(
                solicitanteId,
                "bloqueio_seguranca",
                $"Tentativa de cancelamento não autorizado da reserva #{reservaId}.");
            throw new UnauthorizedAccessException("Você não tem permissão para cancelar esta reserva.");
        }

        if (reserva.Status == "cancelada") return true;

        var atualizado = await reservaRepo.UpdateStatusAsync(reservaId, "cancelada");
        if (atualizado)
        {
            // Restaura a vaga na carona de forma atômica
            await caronaRepo.IncrementVagaAsync(reserva.CaronaId);

            await auditClient.LogAsync(
                solicitanteId,
                "reserva_cancelada",
                $"Reserva #{reservaId} cancelada. Vaga restaurada para carona #{reserva.CaronaId}.");
        }

        return atualizado;
    }

    public async Task<IEnumerable<Reserva>> ListarMinhasReservasAsync(int passageiroId)
    {
        return await reservaRepo.GetByPassageiroIdAsync(passageiroId);
    }
}
