namespace Core.Application.DTOs;

/// <summary>
/// DTO para listagem simplificada de clientes
/// </summary>
public class ClienteListDto
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
    /// Cidade do cliente
    /// </summary>
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o cliente está ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data da criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; }
}
