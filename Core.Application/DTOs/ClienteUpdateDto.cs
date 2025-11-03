namespace Core.Application.DTOs;

/// <summary>
/// DTO para atualização de um cliente existente
/// </summary>
public class ClienteUpdateDto
{
    /// <summary>
    /// Identificador único do cliente
    /// </summary>
    public Guid Id { get; set; }

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
}
