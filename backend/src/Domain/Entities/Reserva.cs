namespace Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int CaronaId { get; set; }
    public int PassageiroId { get; set; }
    public string Status { get; set; } = "confirmada";
    public DateTime CreatedAt { get; set; }

    // Propriedades auxiliares de exibição
    public string? PassageiroNome { get; set; }
    public string? CidadeOrigem { get; set; }
    public string? CidadeDestino { get; set; }
    public DateTime? DataHoraSaida { get; set; }
    public decimal? ValorRateio { get; set; }
}
