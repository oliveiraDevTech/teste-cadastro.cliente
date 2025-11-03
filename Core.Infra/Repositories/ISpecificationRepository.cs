namespace Core.Infra.Repositories;

/// <summary>
/// Interface para repositório com suporte a specifications (padrão Specification)
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface ISpecificationRepository<T> where T : class
{
    /// <summary>
    /// Obtém entidades usando uma specification
    /// </summary>
    Task<IEnumerable<T>> GetAsync(ISpecification<T> specification);

    /// <summary>
    /// Obtém a primeira entidade usando uma specification
    /// </summary>
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification);

    /// <summary>
    /// Conta entidades usando uma specification
    /// </summary>
    Task<int> CountAsync(ISpecification<T> specification);
}

/// <summary>
/// Interface para specification (define filtros, ordenação, includes)
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Critério de filtro
    /// </summary>
    Func<T, bool>? Criteria { get; }

    /// <summary>
    /// Propriedades para incluir (eager loading)
    /// </summary>
    List<Func<T, bool>> IncludeStrings { get; }

    /// <summary>
    /// Ordenação
    /// </summary>
    Func<IEnumerable<T>, IOrderedEnumerable<T>>? OrderBy { get; }

    /// <summary>
    /// Número de registros a pular
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Número de registros a retornar
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Indica se deve usar paginação
    /// </summary>
    bool IsPagingEnabled { get; }
}
