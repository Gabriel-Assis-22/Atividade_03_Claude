namespace Domain.Entities;

public class Avaliacao
{
    public int Id { get; set; }
    public int CaronaId { get; set; }
    public int AvaliadorId { get; set; }
    public int AvaliadoId { get; set; }
    public int Nota { get; set; }
    public string? Comentario { get; set; }
    public DateTime CreatedAt { get; set; }
}
