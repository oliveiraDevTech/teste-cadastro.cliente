using Core.Domain.Common;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade de domínio para Informações Financeiras do Cliente
/// Armazena dados financeiros e análise de crédito
/// </summary>
public class InformacoesFinanceirasCliente : BaseEntity
{
    /// <summary>
    /// ID do cliente relacionado
    /// </summary>
    public Guid ClienteId { get; private set; }

    /// <summary>
    /// Renda estimada do cliente em reais
    /// </summary>
    public decimal Renda { get; private set; } = 0;

    /// <summary>
    /// Renda comprovada do cliente (ex: salário formal)
    /// </summary>
    public decimal RendaComprovada { get; private set; } = 0;

    /// <summary>
    /// Score de crédito (0-1000)
    /// </summary>
    public int ScoreCredito { get; private set; } = 0;

    /// <summary>
    /// Ranking de crédito (0-5)
    /// 0 = Sem avaliação
    /// 1 = Muito Ruim
    /// 2 = Ruim
    /// 3 = Aceitável
    /// 4 = Bom
    /// 5 = Excelente
    /// </summary>
    public int RankingCredito { get; private set; } = 0;

    /// <summary>
    /// Limite de crédito sugerido em reais
    /// </summary>
    public decimal LimiteCreditoSugerido { get; private set; } = 0;

    /// <summary>
    /// Limite de crédito ativo/aprovado em reais
    /// </summary>
    public decimal LimiteCreditoAtivo { get; private set; } = 0;

    /// <summary>
    /// Total de débitos em nome do cliente
    /// </summary>
    public decimal TotalDebitos { get; private set; } = 0;

    /// <summary>
    /// Número de créditos em aberto
    /// </summary>
    public int CreditosEmAberto { get; private set; } = 0;

    /// <summary>
    /// Número de atrasos nos últimos 12 meses
    /// </summary>
    public int AtrasosDiversos12Meses { get; private set; } = 0;

    /// <summary>
    /// Indicador se tem cartão de crédito aprovado
    /// </summary>
    public bool AptoParaCartaoCredito { get; private set; } = false;

    /// <summary>
    /// Tipos de cartões já emitidos (separado por vírgula)
    /// Ex: "VISA,MASTERCARD,ELO"
    /// </summary>
    public string CartoesEmitidos { get; private set; } = string.Empty;

    /// <summary>
    /// Data da última análise de crédito
    /// </summary>
    public DateTime? DataUltimaAnalise { get; private set; }

    /// <summary>
    /// Data da próxima análise de crédito recomendada
    /// </summary>
    public DateTime? DataProximaAnaliseRecomendada { get; private set; }

    /// <summary>
    /// Motivo da recusa (se houver)
    /// </summary>
    public string? MotivoRecusa { get; private set; }

    /// <summary>
    /// Análise textual do resultado da avaliação
    /// </summary>
    public string AnaliseRiscoCredito { get; private set; } = string.Empty;

    /// <summary>
    /// Recomendações para melhorar o score (separado por |)
    /// </summary>
    public string Recomendacoes { get; private set; } = string.Empty;

    /// <summary>
    /// Construtor privado
    /// </summary>
    private InformacoesFinanceirasCliente() { }

    /// <summary>
    /// Cria uma nova instância de Informações Financeiras
    /// </summary>
    public static InformacoesFinanceirasCliente Criar(Guid clienteId, decimal renda, decimal rendaComprovada)
    {
        // Validações rápidas
        if (clienteId == Guid.Empty)
            throw new ArgumentException("ClienteId não pode ser vazio");

        if (renda < 0)
            throw new ArgumentException("Renda não pode ser negativa");

        if (rendaComprovada < 0)
            throw new ArgumentException("Renda comprovada não pode ser negativa");

        return new InformacoesFinanceirasCliente
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Renda = renda,
            RendaComprovada = rendaComprovada,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };
    }

    /// <summary>
    /// Atualiza a renda do cliente
    /// </summary>
    public void AtualizarRenda(decimal renda, decimal rendaComprovada, string? atualizadoPor = null)
    {
        if (renda < 0)
            throw new ArgumentException("Renda não pode ser negativa");

        if (rendaComprovada < 0)
            throw new ArgumentException("Renda comprovada não pode ser negativa");

        Renda = renda;
        RendaComprovada = rendaComprovada;
        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Registra análise de crédito do cliente
    /// </summary>
    public void RegistrarAnaliseCredito(
        int scoreCredito,
        int rankingCredito,
        decimal limiteSugerido,
        bool aptoParaCartao,
        string? motiveRecusa = null,
        string analise = "",
        string recomendacoes = "",
        string? atualizadoPor = null)
    {
        // Validações
        if (scoreCredito < 0 || scoreCredito > 1000)
            throw new ArgumentException("Score de crédito deve estar entre 0 e 1000");

        if (rankingCredito < 0 || rankingCredito > 5)
            throw new ArgumentException("Ranking de crédito deve estar entre 0 e 5");

        if (limiteSugerido < 0)
            throw new ArgumentException("Limite sugerido não pode ser negativo");

        ScoreCredito = scoreCredito;
        RankingCredito = rankingCredito;
        LimiteCreditoSugerido = limiteSugerido;
        AptoParaCartaoCredito = aptoParaCartao;
        MotivoRecusa = motiveRecusa;
        AnaliseRiscoCredito = analise;
        Recomendacoes = recomendacoes;
        DataUltimaAnalise = DateTime.UtcNow;
        DataProximaAnaliseRecomendada = CalcularProximaDataAnalise(rankingCredito);

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Aprova limite de crédito para o cliente
    /// </summary>
    public void AprovarLimiteCredito(decimal limiteAprovado, string? atualizadoPor = null)
    {
        if (limiteAprovado <= 0)
            throw new ArgumentException("Limite aprovado deve ser maior que zero");

        if (limiteAprovado > LimiteCreditoSugerido)
            throw new ArgumentException("Limite aprovado não pode ser superior ao limite sugerido");

        LimiteCreditoAtivo = limiteAprovado;
        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Registra emissão de cartão
    /// </summary>
    public void RegistrarEmissaoCartao(string tipoCartao, string? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(tipoCartao))
            throw new ArgumentException("Tipo de cartão não pode ser vazio");

        // Adiciona novo tipo se não existir
        if (!CartoesEmitidos.Contains(tipoCartao))
        {
            CartoesEmitidos = string.IsNullOrEmpty(CartoesEmitidos)
                ? tipoCartao
                : $"{CartoesEmitidos},{tipoCartao}";
        }

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Atualiza informações de débitos do cliente
    /// </summary>
    public void AtualizarDebitos(decimal totalDebitos, int creditosEmAberto, int atrasosDiversos12Meses, string? atualizadoPor = null)
    {
        if (totalDebitos < 0)
            throw new ArgumentException("Total de débitos não pode ser negativo");

        if (creditosEmAberto < 0)
            throw new ArgumentException("Créditos em aberto não pode ser negativo");

        if (atrasosDiversos12Meses < 0)
            throw new ArgumentException("Atrasos diversos não pode ser negativo");

        TotalDebitos = totalDebitos;
        CreditosEmAberto = creditosEmAberto;
        AtrasosDiversos12Meses = atrasosDiversos12Meses;

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Calcula a próxima data recomendada para análise de crédito
    /// </summary>
    private static DateTime CalcularProximaDataAnalise(int rankingCredito)
    {
        // Clientes com ranking melhor são analisados menos frequentemente
        int diasAteProximaAnalise = rankingCredito switch
        {
            5 => 365, // Excelente: 1 ano
            4 => 270, // Bom: 9 meses
            3 => 180, // Aceitável: 6 meses
            2 => 90,  // Ruim: 3 meses
            1 => 30,  // Muito Ruim: 1 mês
            _ => 15   // Sem avaliação: 15 dias
        };

        return DateTime.UtcNow.AddDays(diasAteProximaAnalise);
    }

    /// <summary>
    /// Verifica se a análise de crédito está expirada e necessita atualização
    /// </summary>
    public bool AnaliseExpirada()
    {
        if (DataProximaAnaliseRecomendada == null)
            return true;

        return DateTime.UtcNow > DataProximaAnaliseRecomendada;
    }

    /// <summary>
    /// Obtém descrição do ranking de crédito
    /// </summary>
    public string ObterDescricaoRanking()
    {
        return RankingCredito switch
        {
            0 => "Sem avaliação",
            1 => "Muito Ruim - Score baixo",
            2 => "Ruim - Risco alto",
            3 => "Aceitável - Pode emitir cartão",
            4 => "Bom - Crédito aprovado",
            5 => "Excelente - Crédito premium",
            _ => "Desconhecido"
        };
    }

    /// <summary>
    /// Calcula a capacidade de pagamento estimada
    /// </summary>
    public decimal CalcularCapacidadePagamento()
    {
        // Usa 30% da renda comprovada como capacidade de pagamento
        // Se não houver renda comprovada, usa 20% da renda estimada
        decimal rendaParaCalculo = RendaComprovada > 0 ? RendaComprovada : Renda;
        return Math.Max(0, (rendaParaCalculo * 0.30m) - (TotalDebitos / 12));
    }

    /// <summary>
    /// Verifica se cliente está em situação de risco
    /// </summary>
    public bool EstaEmRisco()
    {
        // Em risco se: score baixo OU muitos atrasos OU capacidade negativa
        return ScoreCredito < 600 || AtrasosDiversos12Meses >= 3 || CalcularCapacidadePagamento() < 0;
    }
}
