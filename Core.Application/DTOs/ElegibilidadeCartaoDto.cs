namespace Core.Application.DTOs;

/// <summary>
/// DTO para verificar elegibilidade de um cliente para cartão de crédito
/// </summary>
public class ElegibilidadeCartaoDto
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Indica se o cliente é elegível para cartão
    /// </summary>
    public bool ElegivelParaCartao { get; set; }

    /// <summary>
    /// Score de crédito do cliente
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Ranking de crédito do cliente (1-5)
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Motivo da decisão
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição textual do ranking
    /// </summary>
    public string DescricaoRanking { get; set; } = string.Empty;

    /// <summary>
    /// Data da última análise de crédito
    /// </summary>
    public DateTime? DataAnaliseCredito { get; set; }
}

/// <summary>
/// DTO para solicitar emissão de cartão
/// </summary>
public class SolicitarEmissaoCartaoDto
{
    /// <summary>
    /// ID do cliente solicitando o cartão
    /// </summary>
    public Guid ClienteId { get; set; }
}

/// <summary>
/// DTO para resposta de solicitação de emissão
/// </summary>
public class ResultadoEmissaoCartaoDto
{
    /// <summary>
    /// Indica se a solicitação foi aceita
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Mensagem de resposta
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// ID do cartão se emitido com sucesso
    /// </summary>
    public Guid? CartaoId { get; set; }

    /// <summary>
    /// Motivo se rejeitado
    /// </summary>
    public string? Motivo { get; set; }

    /// <summary>
    /// Tempo estimado para processamento
    /// </summary>
    public string? TempoEstimado { get; set; } = "Processando...";
}
