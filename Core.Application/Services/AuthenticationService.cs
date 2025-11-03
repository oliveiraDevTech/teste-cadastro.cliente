using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Application.Services;

/// <summary>
/// Serviço de autenticação com JWT
/// Implementa geração e validação de tokens JWT
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    /// <summary>
    /// Usuário padrão para autenticação
    /// </summary>
    private const string DefaultUser = "user";

    /// <summary>
    /// Senha padrão para autenticação
    /// </summary>
    private const string DefaultPassword = "password";

    /// <summary>
    /// Construtor do serviço de autenticação
    /// </summary>
    /// <param name="jwtSecret">Chave secreta para gerar tokens JWT</param>
    /// <param name="jwtIssuer">Emissor do token (issuer)</param>
    /// <param name="jwtAudience">Audiência do token</param>
    /// <param name="jwtExpirationMinutes">Tempo de expiração em minutos</param>
    public AuthenticationService(
        string jwtSecret,
        string jwtIssuer = "CadastroClientesApi",
        string jwtAudience = "CadastroClientesApp",
        int jwtExpirationMinutes = 60)
    {
        _jwtSecret = jwtSecret ?? throw new ArgumentNullException(nameof(jwtSecret));
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
        _jwtExpirationMinutes = jwtExpirationMinutes;
    }

    /// <summary>
    /// Realiza autenticação do usuário
    /// </summary>
    public async Task<ApiResponseDto<LoginResponseDto>> AutenticarAsync(LoginDto login)
    {
        try
        {
            // Validar credenciais
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Senha))
            {
                return new ApiResponseDto<LoginResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Usuário ou senha inválidos",
                    Erros = new List<string> { "Usuário e senha são obrigatórios" }
                };
            }

            // Verificar credenciais (implementação simplificada)
            if (login.Usuario != DefaultUser || login.Senha != DefaultPassword)
            {
                return new ApiResponseDto<LoginResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Usuário ou senha inválidos",
                    Erros = new List<string> { "Credenciais fornecidas não conferem" }
                };
            }

            // Gerar token JWT
            var token = GerarToken(login.Usuario);

            return new ApiResponseDto<LoginResponseDto>
            {
                Sucesso = true,
                Mensagem = "Autenticação realizada com sucesso",
                Dados = new LoginResponseDto
                {
                    Token = token,
                    TipoToken = "Bearer",
                    ExpiracaoEm = _jwtExpirationMinutes * 60, // converter para segundos
                    Mensagem = "Autenticação realizada com sucesso"
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<LoginResponseDto>
            {
                Sucesso = false,
                Mensagem = "Erro ao autenticar",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    public bool ValidarToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extrai as claims do token JWT
    /// </summary>
    public Dictionary<string, string> ExtrairClaimsDoToken(string token)
    {
        var claims = new Dictionary<string, string>();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null)
            {
                foreach (var claim in jwtToken.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }
            }
        }
        catch
        {
            // Retorna dicionário vazio em caso de erro
        }

        return claims;
    }

    /// <summary>
    /// Gera um token JWT
    /// </summary>
    private string GerarToken(string usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario),
                new Claim(ClaimTypes.Name, usuario),
                new Claim("Usuario", usuario)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
