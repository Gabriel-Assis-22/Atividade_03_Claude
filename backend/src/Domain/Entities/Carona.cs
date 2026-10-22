namespace Domain.Entities;

public class Carona
{
    public int Id { get; set; }
    public int MotoristaId { get; set; }
    public int VeiculoId { get; set; }
    public string CidadeOrigem { get; set; } = string.Empty;
    public string CidadeDestino { get; set; } = string.Empty;
    public string PontoEncontro { get; set; } = string.Empty;
    public DateTime DataHoraSaida { get; set; }
    public int VagasTotais { get; set; }
    public int VagasDisponiveis { get; set; }
    public decimal ValorRateio { get; set; }
    public string Status { get; set; } = "disponivel";
    public DateTime CreatedAt { get; set; }

    // Propriedades auxiliares de exibição (Join com Motorista / Veículo)
    public string? MotoristaNome { get; set; }
    public string? MotoristaFotoUrl { get; set; }
    public string? VeiculoModelo { get; set; }
    public string? VeiculoPlaca { get; set; }
}
