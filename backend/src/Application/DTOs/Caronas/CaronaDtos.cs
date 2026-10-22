namespace Application.DTOs.Caronas;

public record CriarCaronaDto(
    int VeiculoId,
    string CidadeOrigem,
    string CidadeDestino,
    string PontoEncontro,
    DateTime DataHoraSaida,
    int VagasTotais,
    decimal ValorRateio
);

public record BuscarCaronaQuery(
    string? Origem,
    string? Destino,
    DateTime? Data
);
