namespace Core.Application.DTOs;

/// <summary>
/// DTO para criação de um novo cliente
/// </summary>
public class ClienteCreateDto
{
    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do cliente
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do cliente
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// CPF do cliente (formato: XXX.XXX.XXX-XX)
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Endereço do cliente
    /// </summary>
    public string Endereco { get; set; } = string.Empty;

    /// <summary>
    /// Cidade do cliente
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Estado/UF do cliente
    /// </summary>
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// CEP do cliente (formato: XXXXX-XXX)
    /// </summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Data de nascimento do cliente
    /// </summary>
    public DateTime? DataNascimento { get; set; }

    /// <summary>
    /// Renda mensal do cliente
    /// </summary>
    public decimal? RendaMensal { get; set; }

    /// <summary>
    /// Histórico de crédito: BOM, REGULAR, RUIM
    /// </summary>
    public string? HistoricoCredito { get; set; }
}
