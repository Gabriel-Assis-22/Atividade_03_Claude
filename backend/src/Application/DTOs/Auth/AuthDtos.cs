namespace Application.DTOs.Auth;

public record LoginRequest(string Email, string Senha);
public record RegisterRequest(string Nome, string Email, string Senha, string Role = "usuario");
public record AuthResponse(string Token, string Nome, string Email, string Role);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NovaSenha);
