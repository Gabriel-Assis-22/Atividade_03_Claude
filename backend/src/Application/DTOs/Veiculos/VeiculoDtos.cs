namespace Application.DTOs.Veiculos;

public record CriarVeiculoDto(
    string Modelo,
    string Placa,
    string Cor,
    int Ano,
    string? FotoUrl
);
