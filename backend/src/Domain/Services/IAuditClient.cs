namespace Domain.Services;

public interface IAuditClient
{
    Task LogAsync(int? usuarioId, string acao, string? detalhes = null, string? ipOrigem = null);
}

