using Driven.RabbitMQ.Events;
using Microsoft.Extensions.Logging;
using Core.Domain.Entities;

namespace Core.Application.Handlers;

/// <summary>
/// Handler para processar eventos de conclusão de análise de crédito
/// Consome AnaliseCartaoCreditoCompleteEvent do serviço de Creditos
/// Atualiza os dados de crédito do cliente no banco
/// </summary>
public class AnaliseCartaoCreditoCompleteEventHandler
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<AnaliseCartaoCreditoCompleteEventHandler> _logger;

    public AnaliseCartaoCreditoCompleteEventHandler(
        IClienteRepository clienteRepository,
        ILogger<AnaliseCartaoCreditoCompleteEventHandler> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
    }

    /// <summary>
    /// Processa o evento de análise de crédito completa
    /// Atualiza os dados de crédito do cliente
    /// </summary>
    public async Task HandleAsync(AnaliseCartaoCreditoCompleteEvent evento)
    {
        try
        {
            _logger.LogInformation(
                "Processando análise de crédito para cliente {ClienteId}. " +
                "Score: {Score}, Ranking: {Ranking}, Elegível: {Elegivel}",
                evento.ClienteId, evento.ScoreCredito, evento.RankingCredito, evento.ElegivelCartao);

            // Validar dados do evento
            ValidarEvento(evento);

            // Buscar cliente no banco
            var clienteDto = await _clienteRepository.ObterPorIdAsync(evento.ClienteId);

            if (clienteDto == null)
            {
                _logger.LogWarning(
                    "Cliente {ClienteId} não encontrado na análise de crédito",
                    evento.ClienteId);
                return;
            }

            // Converter DTO para entidade e atualizar dados de crédito
            var clienteUpdateDto = new ClienteUpdateDto
            {
                Id = clienteDto.Id,
                Nome = clienteDto.Nome,
                Email = clienteDto.Email,
                Telefone = clienteDto.Telefone,
                Endereco = clienteDto.Endereco,
                Cidade = clienteDto.Cidade,
                Estado = clienteDto.Estado,
                Cep = clienteDto.Cep,
                RankingCredito = evento.RankingCredito,
                ScoreCredito = evento.ScoreCredito
            };

            // Persistir alterações
            await _clienteRepository.AtualizarAsync(clienteUpdateDto);

            _logger.LogInformation(
                "Dados de crédito atualizados para cliente {ClienteId}. " +
                "Novo status - Score: {Score}, Ranking: {Ranking}, Elegível: {Elegivel}",
                evento.ClienteId, evento.ScoreCredito, evento.RankingCredito, evento.ElegivelCartao);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar análise de crédito para cliente {ClienteId}",
                evento.ClienteId);
            throw;
        }
    }

    /// <summary>
    /// Valida os dados do evento
    /// </summary>
    private void ValidarEvento(AnaliseCartaoCreditoCompleteEvent evento)
    {
        if (evento.ClienteId == Guid.Empty)
            throw new ArgumentException("ClienteId não pode ser vazio");

        if (evento.ScoreCredito < 0 || evento.ScoreCredito > 1000)
            throw new ArgumentException("Score de crédito deve estar entre 0 e 1000");

        if (evento.RankingCredito < 1 || evento.RankingCredito > 5)
            throw new ArgumentException("Ranking de crédito deve estar entre 1 e 5");

        if (string.IsNullOrWhiteSpace(evento.Motivo))
            throw new ArgumentException("Motivo da análise não pode ser vazio");
    }
}
