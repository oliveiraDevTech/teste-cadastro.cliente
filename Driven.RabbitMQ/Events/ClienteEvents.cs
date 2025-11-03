namespace Driven.RabbitMQ.Events;

/// <summary>
/// Evento disparado quando um cliente é criado
/// </summary>
public class ClienteCreatedEvent : DomainEvent
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
}

/// <summary>
/// Evento disparado quando um cliente é atualizado
/// </summary>
public class ClienteUpdatedEvent : DomainEvent
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public DateTime DataAtualizacao { get; set; }
}

/// <summary>
/// Evento disparado quando um cliente é deletado
/// </summary>
public class ClienteDeletedEvent : DomainEvent
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool SoftDelete { get; set; } = true;
}

/// <summary>
/// Evento disparado quando um cliente é reativado
/// </summary>
public class ClienteActivatedEvent : DomainEvent
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Evento de resposta com análise de crédito do cliente
/// Retornado pela fila de análise de cartão de crédito
/// </summary>
public class ClienteAnaliseCartaoCreditoResponseEvent : DomainEvent
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
    public DateTime DataAnalise { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tempo de processamento em ms
    /// </summary>
    public long TempoProcessamentoDes { get; set; }
}
