namespace Core.Application.DTOs;

/// <summary>
/// DTO para respostas paginadas de listagens
/// </summary>
/// <typeparam name="T">Tipo dos itens na paginação</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>
    /// Número da página atual (começando em 1)
    /// </summary>
    public int PaginaAtual { get; set; }

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int ItensPorPagina { get; set; }

    /// <summary>
    /// Número total de páginas
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Total de itens encontrados
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Lista dos itens da página atual
    /// </summary>
    public List<T> Itens { get; set; } = new();

    /// <summary>
    /// Indica se existe próxima página
    /// </summary>
    public bool TemProxima => PaginaAtual < TotalPaginas;

    /// <summary>
    /// Indica se existe página anterior
    /// </summary>
    public bool TemAnterior => PaginaAtual > 1;
}
