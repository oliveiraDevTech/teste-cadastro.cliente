namespace Core.Infra.Repositories;

/// <summary>
/// Implementação base de uma specification
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public class Specification<T> : ISpecification<T> where T : class
{
    public Func<T, bool>? Criteria { get; set; }
    public List<Func<T, bool>> IncludeStrings { get; } = new();
    public Func<IEnumerable<T>, IOrderedEnumerable<T>>? OrderBy { get; set; }
    public int? Take { get; set; }
    public int? Skip { get; set; }
    public bool IsPagingEnabled { get; set; }

    protected virtual void AddInclude(Func<T, bool> includeExpression)
    {
        IncludeStrings.Add(includeExpression);
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected virtual void ApplyOrdering(Func<IEnumerable<T>, IOrderedEnumerable<T>> orderingExpression)
    {
        OrderBy = orderingExpression;
    }
}

/// <summary>
/// Specification para buscar por ID
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public class ById<T> : Specification<T> where T : class
{
    public ById(Guid id)
    {
        // Esta é uma specification genérica
        // A lógica específica de ID dependerá da implementação do repositório
    }
}

/// <summary>
/// Specification com filtros comuns
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public class PagedSpecification<T> : Specification<T> where T : class
{
    public PagedSpecification(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
