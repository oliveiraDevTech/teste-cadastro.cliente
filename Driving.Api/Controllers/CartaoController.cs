using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Application.DTOs;
using Driven.RabbitMQ.Events;
using Driven.RabbitMQ.Interfaces;

namespace Driving.Api.Controllers;

/// <summary>
/// Controller para operações relacionadas a cartão de crédito
/// Gerencia elegibilidade e solicitações de emissão
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartaoController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<CartaoController> _logger;
    private readonly Driven.RabbitMQ.Settings.RabbitMQSettings _rabbitMQSettings;

    public CartaoController(
        IClienteService clienteService,
        IMessagePublisher messagePublisher,
        ILogger<CartaoController> logger,
        IOptions<Driven.RabbitMQ.Settings.RabbitMQSettings> rabbitMQSettings)
    {
        _clienteService = clienteService;
        _messagePublisher = messagePublisher;
        _logger = logger;
        _rabbitMQSettings = rabbitMQSettings.Value;
    }

    /// <summary>
    /// Verifica se um cliente é elegível para emitir cartão de crédito
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Informações sobre elegibilidade</returns>
    /// <response code="200">Cliente consultado com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("{clienteId}/elegibilidade-cartao")]
    [ProducesResponseType(typeof(ApiResponseDto<ElegibilidadeCartaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerificarElegibilidadeAsync(Guid clienteId)
    {
        try
        {
            _logger.LogInformation("Verificando elegibilidade para cartão do cliente {ClienteId}", clienteId);

            // Validar ID
            if (clienteId == Guid.Empty)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente inválido",
                    Erros = new List<string> { "ClienteId não pode estar vazio" }
                });
            }

            // Buscar cliente
            var resultadoCliente = await _clienteService.ObterPorIdAsync(clienteId);

            if (!resultadoCliente.Sucesso || resultadoCliente.Dados == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { $"Nenhum cliente encontrado com ID {clienteId}" }
                });
            }

            var cliente = resultadoCliente.Dados;

            // Obter descrição do ranking
            string descricaoRanking = cliente.RankingCredito switch
            {
                0 => "Sem avaliação",
                1 => "Muito Ruim - Score baixo",
                2 => "Ruim - Risco alto",
                3 => "Aceitável - Pode emitir cartão",
                4 => "Bom - Crédito aprovado",
                5 => "Excelente - Crédito premium",
                _ => "Desconhecido"
            };

            // Preparar resposta de elegibilidade
            var elegibilidade = new ElegibilidadeCartaoDto
            {
                ClienteId = cliente.Id,
                ElegivelParaCartao = cliente.AptoParaCartaoCredito,
                ScoreCredito = cliente.ScoreCredito,
                RankingCredito = cliente.RankingCredito,
                DataAnaliseCredito = cliente.DataAtualizacaoRanking,
                DescricaoRanking = descricaoRanking,
                Motivo = cliente.AptoParaCartaoCredito
                    ? $"Cliente elegível (Score: {cliente.ScoreCredito}, Ranking: {cliente.RankingCredito})"
                    : "Cliente não atende aos requisitos mínimos para emissão de cartão"
            };

            return Ok(new ApiResponseDto<ElegibilidadeCartaoDto>
            {
                Sucesso = true,
                Mensagem = "Elegibilidade verificada com sucesso",
                Dados = elegibilidade
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar elegibilidade para cartão do cliente {ClienteId}", clienteId);

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<object>
            {
                Sucesso = false,
                Mensagem = "Erro ao verificar elegibilidade",
                Erros = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Solicita a emissão de um cartão de crédito para um cliente
    /// Verifica elegibilidade e publica evento para processamento assíncrono
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Resultado da solicitação</returns>
    /// <response code="202">Solicitação aceita para processamento</response>
    /// <response code="400">ID inválido ou cliente não elegível</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("{clienteId}/emitir-cartao")]
    [ProducesResponseType(typeof(ApiResponseDto<ResultadoEmissaoCartaoDto>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SolicitarEmissaoCartaoAsync(Guid clienteId)
    {
        try
        {
            _logger.LogInformation("Solicitação de emissão de cartão para cliente {ClienteId}", clienteId);

            // Validar ID
            if (clienteId == Guid.Empty)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente inválido",
                    Erros = new List<string> { "ClienteId não pode estar vazio" }
                });
            }

            // Buscar cliente
            var resultadoCliente = await _clienteService.ObterPorIdAsync(clienteId);

            if (!resultadoCliente.Sucesso || resultadoCliente.Dados == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { $"Nenhum cliente encontrado com ID {clienteId}" }
                });
            }

            var cliente = resultadoCliente.Dados;

            // Verificar elegibilidade
            if (!cliente.AptoParaCartaoCredito)
            {
                return BadRequest(new ApiResponseDto<ResultadoEmissaoCartaoDto>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não elegível para emissão de cartão",
                    Dados = new ResultadoEmissaoCartaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Cliente não atende aos requisitos mínimos",
                        Motivo = $"Score insuficiente: {cliente.ScoreCredito} (mínimo 600) ou Ranking: {cliente.RankingCredito} (mínimo 3)"
                    }
                });
            }

            // Publicar evento de pedido de emissão
            var eventoEmissao = new PedidoEmissaoCartaoIntegrationEvent
            {
                ClienteId = cliente.Id,
                PropostaId = Guid.NewGuid(),
                ContaId = Guid.NewGuid(),
                CodigoProduto = "CREDIT_CARD_PLATINUM",
                QuantidadeCartoesEmitir = cliente.ScoreCredito >= 501 ? 2 : 1,
                LimiteCreditoPorCartao = (cliente.ScoreCredito * 10) / (cliente.ScoreCredito >= 501 ? 2 : 1),
                CorrelacaoId = Guid.NewGuid().ToString(),
                ChaveIdempotencia = $"{cliente.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                Entrega = new EntregaCartaoInfo
                {
                    TipoEntrega = "CORREIOS_SEDEX",
                    EnderecoEntrega = new EnderecoEntregaInfo
                    {
                        Logradouro = cliente.Endereco,
                        Cidade = cliente.Cidade,
                        Estado = cliente.Estado,
                        Cep = cliente.Cep
                    }
                },
                DataSolicitacao = DateTime.UtcNow
            };

            await _messagePublisher.PublishAsync(_rabbitMQSettings.Queues.CartaoEmissaoPedido, eventoEmissao);

            _logger.LogInformation(
                "Evento PedidoEmissaoCartaoIntegrationEvent publicado para cliente {ClienteId}",
                clienteId);

            // Retornar sucesso com status 202 Accepted (processamento assíncrono)
            return Accepted(new ApiResponseDto<ResultadoEmissaoCartaoDto>
            {
                Sucesso = true,
                Mensagem = "Solicitação de emissão de cartão recebida e será processada",
                Dados = new ResultadoEmissaoCartaoDto
                {
                    Sucesso = true,
                    Mensagem = "Seu cartão está sendo processado e será enviado em breve",
                    CartaoId = null, // Será preenchido após emissão
                    TempoEstimado = "Processando... (aproximadamente 2-5 minutos)"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar solicitação de emissão de cartão para cliente {ClienteId}",
                clienteId);

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<object>
            {
                Sucesso = false,
                Mensagem = "Erro ao processar solicitação de emissão",
                Erros = new List<string> { ex.Message }
            });
        }
    }
}
