namespace Core.Application.DTOs;

/// <summary>
/// DTO para criação de informações financeiras do cliente
/// </summary>
public class CriarInformacoesFinanceirasDto
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Renda estimada em reais
    /// </summary>
    public decimal Renda { get; set; }

    /// <summary>
    /// Renda comprovada em reais
    /// </summary>
    public decimal RendaComprovada { get; set; }
}

/// <summary>
/// DTO para atualização de renda
/// </summary>
public class AtualizarRendaDto
{
    /// <summary>
    /// Renda estimada
    /// </summary>
    public decimal Renda { get; set; }

    /// <summary>
    /// Renda comprovada
    /// </summary>
    public decimal RendaComprovada { get; set; }
}

/// <summary>
/// DTO para atualização de análise de crédito
/// </summary>
public class RegistrarAnaliseCreditoDto
{
    /// <summary>
    /// Score de crédito (0-1000)
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Ranking de crédito (0-5)
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Limite sugerido de crédito
    /// </summary>
    public decimal LimiteSugerido { get; set; }

    /// <summary>
    /// Se está apto para cartão
    /// </summary>
    public bool AptoParaCartao { get; set; }

    /// <summary>
    /// Motivo da recusa (se aplicável)
    /// </summary>
    public string? MotivoRecusa { get; set; }

    /// <summary>
    /// Análise textual
    /// </summary>
    public string Analise { get; set; } = string.Empty;

    /// <summary>
    /// Recomendações
    /// </summary>
    public string Recomendacoes { get; set; } = string.Empty;
}

/// <summary>
/// DTO para aprovação de limite de crédito
/// </summary>
public class AprovarLimiteCreditoDto
{
    /// <summary>
    /// Limite aprovado em reais
    /// </summary>
    public decimal LimiteAprovado { get; set; }
}

/// <summary>
/// DTO para atualização de débitos
/// </summary>
public class AtualizarDebitosDto
{
    /// <summary>
    /// Total de débitos
    /// </summary>
    public decimal TotalDebitos { get; set; }

    /// <summary>
    /// Créditos em aberto
    /// </summary>
    public int CreditosEmAberto { get; set; }

    /// <summary>
    /// Atrasos nos últimos 12 meses
    /// </summary>
    public int AtrasosDiversos12Meses { get; set; }
}

/// <summary>
/// DTO para retorno de informações financeiras do cliente
/// </summary>
public class InformacoesFinanceirasResponseDto
{
    /// <summary>
    /// ID das informações
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Renda estimada
    /// </summary>
    public decimal Renda { get; set; }

    /// <summary>
    /// Renda comprovada
    /// </summary>
    public decimal RendaComprovada { get; set; }

    /// <summary>
    /// Score de crédito
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Ranking de crédito
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Descrição do ranking
    /// </summary>
    public string DescricaoRanking { get; set; } = string.Empty;

    /// <summary>
    /// Limite sugerido
    /// </summary>
    public decimal LimiteCreditoSugerido { get; set; }

    /// <summary>
    /// Limite ativo/aprovado
    /// </summary>
    public decimal LimiteCreditoAtivo { get; set; }

    /// <summary>
    /// Total de débitos
    /// </summary>
    public decimal TotalDebitos { get; set; }

    /// <summary>
    /// Créditos em aberto
    /// </summary>
    public int CreditosEmAberto { get; set; }

    /// <summary>
    /// Atrasos nos últimos 12 meses
    /// </summary>
    public int AtrasosDiversos12Meses { get; set; }

    /// <summary>
    /// Se está apto para cartão
    /// </summary>
    public bool AptoParaCartaoCredito { get; set; }

    /// <summary>
    /// Cartões já emitidos
    /// </summary>
    public string CartoesEmitidos { get; set; } = string.Empty;

    /// <summary>
    /// Data da última análise
    /// </summary>
    public DateTime? DataUltimaAnalise { get; set; }

    /// <summary>
    /// Data próxima análise recomendada
    /// </summary>
    public DateTime? DataProximaAnaliseRecomendada { get; set; }

    /// <summary>
    /// Motivo da recusa
    /// </summary>
    public string? MotivoRecusa { get; set; }

    /// <summary>
    /// Análise de risco
    /// </summary>
    public string AnaliseRiscoCredito { get; set; } = string.Empty;

    /// <summary>
    /// Recomendações
    /// </summary>
    public string Recomendacoes { get; set; } = string.Empty;

    /// <summary>
    /// Capacidade de pagamento
    /// </summary>
    public decimal CapacidadePagamento { get; set; }

    /// <summary>
    /// Se está em risco
    /// </summary>
    public bool EstaEmRisco { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }
}
