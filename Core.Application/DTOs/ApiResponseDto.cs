namespace Core.Application.DTOs;

/// <summary>
/// DTO padrão para respostas da API
/// </summary>
/// <typeparam name="T">Tipo do dado retornado</typeparam>
public class ApiResponseDto<T>
{
    /// <summary>
    /// Indica se a requisição foi bem-sucedida
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Mensagem informativa da resposta
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Dados retornados pela API
    /// </summary>
    public T? Dados { get; set; }

    /// <summary>
    /// Lista de erros em caso de falha
    /// </summary>
    public List<string> Erros { get; set; } = new();

    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Versão simplificada do ApiResponseDto sem tipo genérico
/// </summary>
public class ApiResponseDto
{
    /// <summary>
    /// Indica se a requisição foi bem-sucedida
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Mensagem informativa da resposta
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Lista de erros em caso de falha
    /// </summary>
    public List<string> Erros { get; set; } = new();

    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
