using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Application.DTOs;
using Core.Application.Interfaces.Services;

namespace Driving.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de clientes
/// Todas as ações requerem autenticação com token JWT
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    /// <summary>
    /// Construtor do controller
    /// </summary>
    /// <param name="clienteService">Serviço de clientes injetado por DI</param>
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Obtém um cliente por seu ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Dados completos do cliente</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
    {
        var resultado = await _clienteService.ObterPorIdAsync(id);

        if (!resultado.Sucesso)
        {
            if (resultado.Erros.Any(e => e.Contains("não encontrado")))
                return NotFound(resultado);
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de clientes</returns>
    /// <response code="200">Lista de clientes obtida com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<ClienteListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int itensPorPagina = 10)
    {
        var resultado = await _clienteService.ListarAsync(pagina, itensPorPagina);
        return Ok(resultado);
    }

    /// <summary>
    /// Pesquisa clientes por nome
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do cliente</param>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de clientes que correspondem à busca</returns>
    /// <response code="200">Pesquisa realizada com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<ClienteListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<ClienteListDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] string nome, [FromQuery] int pagina = 1, [FromQuery] int itensPorPagina = 10)
    {
        var resultado = await _clienteService.PesquisarPorNomeAsync(nome, pagina, itensPorPagina);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return Ok(resultado);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="clienteCreateDto">Dados do novo cliente</param>
    /// <returns>Cliente criado com ID gerado</returns>
    /// <response code="201">Cliente criado com sucesso</response>
    /// <response code="400">Dados inválidos ou cliente já existe</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] ClienteCreateDto clienteCreateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Dados inválidos",
                Erros = errors
            });
        }

        var resultado = await _clienteService.CriarAsync(clienteCreateDto);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.Id }, resultado);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    /// <param name="id">ID do cliente a atualizar</param>
    /// <param name="clienteUpdateDto">Dados atualizados do cliente</param>
    /// <returns>Cliente atualizado</returns>
    /// <response code="200">Cliente atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<ClienteResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] ClienteUpdateDto clienteUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Dados inválidos",
                Erros = errors
            });
        }

        // Garantir que o ID da rota corresponde ao ID do DTO
        clienteUpdateDto.Id = id;

        var resultado = await _clienteService.AtualizarAsync(clienteUpdateDto);

        if (!resultado.Sucesso)
        {
            if (resultado.Erros.Any(e => e.Contains("não encontrado") || e.Contains("não existe")))
                return NotFound(resultado);
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Deleta um cliente
    /// </summary>
    /// <param name="id">ID do cliente a deletar</param>
    /// <returns>Confirmação de deleção</returns>
    /// <response code="200">Cliente deletado com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar([FromRoute] Guid id)
    {
        var resultado = await _clienteService.DeletarAsync(id);

        if (!resultado.Sucesso)
        {
            if (resultado.Erros.Any(e => e.Contains("não encontrado") || e.Contains("não existe")))
                return NotFound(resultado);
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }
}
