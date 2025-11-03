namespace Core.Application.DTOs;

/// <summary>
/// DTO para retorno de informações de ranking de crédito
/// </summary>
public class RankingCreditoDto
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Ranking de crédito (1-5)
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Score de crédito (0-1000)
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Descrição do ranking
    /// </summary>
    public string DescricaoRanking { get; set; } = string.Empty;

    /// <summary>
    /// Se está apto a receber cartão de crédito
    /// </summary>
    public bool AptoParaCartao { get; set; }

    /// <summary>
    /// Análise textual do resultado
    /// </summary>
    public string Analise { get; set; } = string.Empty;

    /// <summary>
    /// Motivo da recusa (se não apto)
    /// </summary>
    public string? MotivoDaRecusa { get; set; }

    /// <summary>
    /// Limite sugerido de crédito
    /// </summary>
    public decimal LimiteSugerado { get; set; }

    /// <summary>
    /// Data da última atualização de ranking
    /// </summary>
    public DateTime? DataAtualizacaoRanking { get; set; }
}

/// <summary>
/// DTO para requisição de análise de cartão de crédito
/// </summary>
public class AnaliseCartaoCreditoRequestDto
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Nome do cliente
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do cliente
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// CPF do cliente
    /// </summary>
    public string CPF { get; set; } = string.Empty;

    /// <summary>
    /// Renda estimada do cliente
    /// </summary>
    public decimal Renda { get; set; }

    /// <summary>
    /// Cidade do cliente
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Estado do cliente
    /// </summary>
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de análise de cartão de crédito
/// </summary>
public class AnaliseCartaoCreditoResponseDto
{
    /// <summary>
    /// ID do cliente analisado
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Ranking de crédito (1-5)
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Score de crédito (0-1000)
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Análise textual do resultado
    /// </summary>
    public string Analise { get; set; } = string.Empty;

    /// <summary>
    /// Se está apto a receber cartão de crédito
    /// </summary>
    public bool AptoParaCartao { get; set; }

    /// <summary>
    /// Motivo da recusa (se não apto)
    /// </summary>
    public string? MotivoDaRecusa { get; set; }

    /// <summary>
    /// Limite sugerido de crédito
    /// </summary>
    public decimal LimiteSugerado { get; set; }

    /// <summary>
    /// Data da análise
    /// </summary>
    public DateTime DataAnalise { get; set; }

    /// <summary>
    /// Tempo de processamento em milissegundos
    /// </summary>
    public long TempoProcessamentoMs { get; set; }

    /// <summary>
    /// Recomendações para melhorar o score
    /// </summary>
    public List<string> Recomendacoes { get; set; } = new();
}
