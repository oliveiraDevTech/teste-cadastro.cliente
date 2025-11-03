namespace Core.Application.DTOs;

/// <summary>
/// DTO para autenticação de usuários
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Nome de usuário para autenticação
    /// </summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    public string Senha { get; set; } = string.Empty;
}
