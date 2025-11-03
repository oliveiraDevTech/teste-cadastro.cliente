namespace Core.Application.DTOs;

/// <summary>
/// DTO de resposta com os dados completos de um cliente
/// </summary>
public class ClienteResponseDto
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
    /// CPF do cliente
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
    /// CEP do cliente
    /// </summary>
    public string Cep { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Indica se o cliente está ativo
    /// </summary>
    public bool Ativo { get; set; }
}
