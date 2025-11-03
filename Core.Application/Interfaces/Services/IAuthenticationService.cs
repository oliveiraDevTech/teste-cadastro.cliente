namespace Core.Application.Interfaces.Services;

/// <summary>
/// Interface do serviço de autenticação
/// Define os contratos para operações de autenticação
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Realiza autenticação do usuário
    /// </summary>
    /// <param name="login">Dados de login (usuário e senha)</param>
    /// <returns>Resposta com token JWT em caso de sucesso ou erro de autenticação</returns>
    Task<ApiResponseDto<LoginResponseDto>> AutenticarAsync(LoginDto login);

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT a validar</param>
    /// <returns>True se token é válido, False caso contrário</returns>
    bool ValidarToken(string token);

    /// <summary>
    /// Extrai as claims do token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Dicionário com as claims do token</returns>
    Dictionary<string, string> ExtrairClaimsDoToken(string token);
}
