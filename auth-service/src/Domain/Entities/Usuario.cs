namespace Domain.Entities;

public enum Role { passageiro, motorista, admin, usuario }

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.passageiro;
    public DateTime CriadoEm { get; set; }
}

