namespace Driven.RabbitMQ.Events;

/// <summary>
/// Evento de integração: Cliente foi cadastrado (publicado por este serviço para Creditos)
/// Este evento contém todos os dados necessários para análise de crédito
/// </summary>
public class ClienteCadastradoIntegrationEvent : DomainEvent
{
    /// <summary>
    /// ID único do cliente
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF do cliente
    /// </summary>
    public string CPF { get; set; } = string.Empty;

    /// <summary>
    /// Email do cliente
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Renda mensal em reais
    /// </summary>
    public decimal Renda { get; set; }

    /// <summary>
    /// Idade do cliente
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Histórico de crédito: "BOM", "REGULAR", "RUIM"
    /// </summary>
    public string HistoricoCredito { get; set; } = "REGULAR";

    /// <summary>
    /// Data de nascimento para cálculos complementares
    /// </summary>
    public DateTime DataNascimento { get; set; }
}

/// <summary>
/// Evento de integração: Análise de crédito foi concluída (publicado pelo serviço de Creditos)
/// Este evento é consumido por este serviço para atualizar dados
/// </summary>
public class AnaliseCartaoCreditoCompleteEvent : DomainEvent
{
    /// <summary>
    /// ID do cliente analisado
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Score de crédito calculado (0-1000)
    /// </summary>
    public int ScoreCredito { get; set; }

    /// <summary>
    /// Ranking de crédito (1-5, onde 5 é o melhor)
    /// </summary>
    public int RankingCredito { get; set; }

    /// <summary>
    /// Indica se o cliente é elegível para emitir cartão
    /// </summary>
    public bool ElegivelCartao { get; set; }

    /// <summary>
    /// Motivo da decisão (aprovado, negado, etc.)
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora da análise
    /// </summary>
    public DateTime DataAnalise { get; set; }

    /// <summary>
    /// Limite de crédito recomendado
    /// </summary>
    public decimal LimiteCredito { get; set; }

    /// <summary>
    /// Número máximo de cartões permitidos
    /// </summary>
    public int NumeroMaximoCartoes { get; set; }
}

/// <summary>
/// Evento de integração: Análise de crédito falhou (erro ou rejeição)
/// Publicado pelo serviço de Creditos quando há falha na análise
/// </summary>
public class AnaliseCartaoCreditoFalhouEvent : DomainEvent
{
    /// <summary>
    /// ID do cliente que falhou na análise
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Motivo da falha ou rejeição
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// Data da tentativa de análise
    /// </summary>
    public DateTime DataTentativa { get; set; }

    /// <summary>
    /// Indicador se pode tentar novamente
    /// </summary>
    public bool PoderTentarNovamente { get; set; } = true;
}

/// <summary>
/// Evento de integração: Pedido de emissão de cartão (publicado por este serviço para Cartoes)
/// Enviado quando cliente solicita emissão de cartão
/// </summary>
public class PedidoEmissaoCartaoIntegrationEvent : DomainEvent
{
    /// <summary>
    /// ID do cliente que solicitou o cartão
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// ID da proposta de crédito
    /// </summary>
    public Guid PropostaId { get; set; }

    /// <summary>
    /// ID da conta bancária do cliente
    /// </summary>
    public Guid ContaId { get; set; }

    /// <summary>
    /// Código do produto (ex: CREDIT_CARD_PLATINUM)
    /// </summary>
    public string CodigoProduto { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade de cartões a emitir
    /// </summary>
    public int QuantidadeCartoesEmitir { get; set; }

    /// <summary>
    /// Limite de crédito por cartão
    /// </summary>
    public decimal LimiteCreditoPorCartao { get; set; }

    /// <summary>
    /// ID de correlação para rastreamento
    /// </summary>
    public string CorrelacaoId { get; set; } = string.Empty;

    /// <summary>
    /// Chave de idempotência para evitar duplicatas
    /// </summary>
    public string ChaveIdempotencia { get; set; } = string.Empty;

    /// <summary>
    /// Informações de entrega do cartão
    /// </summary>
    public EntregaCartaoInfo? Entrega { get; set; }

    /// <summary>
    /// Data da solicitação
    /// </summary>
    public DateTime DataSolicitacao { get; set; }
}

/// <summary>
/// Informações de entrega do cartão
/// </summary>
public class EntregaCartaoInfo
{
    /// <summary>
    /// Tipo de entrega (CORREIOS_SEDEX, CORREIOS_PAC, RETIRADA_AGENCIA)
    /// </summary>
    public string TipoEntrega { get; set; } = string.Empty;

    /// <summary>
    /// Endereço para entrega
    /// </summary>
    public EnderecoEntregaInfo? EnderecoEntrega { get; set; }
}

/// <summary>
/// Endereço de entrega
/// </summary>
public class EnderecoEntregaInfo
{
    public string Logradouro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
}

/// <summary>
/// Evento de integração: Cartão foi emitido (publicado pelo serviço de Cartoes)
/// Consumido por este serviço para atualizar dados do cliente
/// </summary>
public class CartaoEmitidoIntegrationEvent : DomainEvent
{
    /// <summary>
    /// ID do cliente que recebeu o cartão
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// ID do cartão emitido
    /// </summary>
    public Guid CartaoId { get; set; }

    /// <summary>
    /// Número do cartão (mascarado/parcial por segurança)
    /// </summary>
    public string NumeroCartao { get; set; } = string.Empty;

    /// <summary>
    /// Status do cartão
    /// </summary>
    public string Status { get; set; } = "EMITIDO";

    /// <summary>
    /// Data de emissão
    /// </summary>
    public DateTime DataEmissao { get; set; }
}

/// <summary>
/// Evento de integração: Emissão de cartão falhou
/// Publicado pelo serviço de Cartoes quando há erro na emissão
/// </summary>
public class CartaoEmissaoFalhouIntegrationEvent : DomainEvent
{
    /// <summary>
    /// ID do cliente que solicitou o cartão
    /// </summary>
    public Guid ClienteId { get; set; }

    /// <summary>
    /// Motivo da falha
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// Data da tentativa
    /// </summary>
    public DateTime DataTentativa { get; set; }
}
