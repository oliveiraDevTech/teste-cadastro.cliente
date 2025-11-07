namespace Core.Application.DTOs;

/// <summary>
/// DTO para dados de cartão retornados do serviço de emissão
/// </summary>
public class CartaoDto
{
    /// <summary>
    /// ID único do cartão
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Número do cartão mascarado (ex: 4111 **** **** 1234)
    /// </summary>
    public string NumeroMascarado { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de cartão (VIRTUAL, PHYSICAL)
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Status do cartão (ISSUED, ACTIVE, BLOCKED, etc)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Limite de crédito disponível
    /// </summary>
    public decimal LimiteCredito { get; set; }

    /// <summary>
    /// Data de emissão do cartão
    /// </summary>
    public DateTime DataEmissao { get; set; }

    /// <summary>
    /// Data de ativação (se já foi ativado)
    /// </summary>
    public DateTime? DataAtivacao { get; set; }
}
