namespace Domain.Entities;

public class Veiculo
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string? FotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
