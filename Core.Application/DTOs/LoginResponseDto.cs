namespace Core.Application.DTOs;

/// <summary>
/// DTO de resposta após autenticação bem-sucedida
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Token JWT para autenticação nas próximas requisições
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do token (geralmente "Bearer")
    /// </summary>
    public string TipoToken { get; set; } = "Bearer";

    /// <summary>
    /// Tempo de expiração do token em segundos
    /// </summary>
    public int ExpiracaoEm { get; set; }

    /// <summary>
    /// Mensagem de sucesso da autenticação
    /// </summary>
    public string Mensagem { get; set; } = "Autenticação realizada com sucesso";
}
