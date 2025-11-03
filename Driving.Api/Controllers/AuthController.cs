using Microsoft.AspNetCore.Mvc;
using Core.Application.DTOs;
using Core.Application.Interfaces.Services;

namespace Driving.Api.Controllers;

/// <summary>
/// Controller para autenticação de usuários
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Construtor do controller
    /// </summary>
    /// <param name="authenticationService">Serviço de autenticação injetado por DI</param>
    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Realiza login do usuário e retorna um token JWT
    /// </summary>
    /// <remarks>
    /// Credenciais padrão para desenvolvimento:
    /// - Usuário: "user"
    /// - Senha: "password"
    /// </remarks>
    /// <param name="login">Dados de login (usuário e senha)</param>
    /// <returns>Token JWT para uso nas requisições subsequentes</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Credenciais inválidas</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(new ApiResponseDto<LoginResponseDto>
            {
                Sucesso = false,
                Mensagem = "Dados de login inválidos",
                Erros = errors
            });
        }

        var resultado = await _authenticationService.AutenticarAsync(login);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return Ok(resultado);
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT a validar</param>
    /// <returns>Indicação se o token é válido</returns>
    /// <response code="200">Token é válido</response>
    /// <response code="400">Token é inválido</response>
    [HttpPost("validar-token")]
    [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status400BadRequest)]
    public IActionResult ValidarToken([FromBody] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new ApiResponseDto
            {
                Sucesso = false,
                Mensagem = "Token é obrigatório",
                Erros = new List<string> { "Token não pode estar vazio" }
            });
        }

        var isValid = _authenticationService.ValidarToken(token);

        if (!isValid)
        {
            return BadRequest(new ApiResponseDto
            {
                Sucesso = false,
                Mensagem = "Token inválido ou expirado",
                Erros = new List<string> { "O token fornecido é inválido ou já expirou" }
            });
        }

        return Ok(new ApiResponseDto
        {
            Sucesso = true,
            Mensagem = "Token é válido"
        });
    }
}
