namespace Core.Infra.Repositories;

/// <summary>
/// Interface para repositório genérico com operações CRUD padrão
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtém uma entidade por ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém todas as entidades
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtém todas as entidades com filtro
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(Func<T, bool> predicate);

    /// <summary>
    /// Encontra a primeira entidade que corresponde ao filtro
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate);

    /// <summary>
    /// Verifica se existe uma entidade que corresponde ao filtro
    /// </summary>
    Task<bool> AnyAsync(Func<T, bool> predicate);

    /// <summary>
    /// Conta o número total de entidades
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Conta o número de entidades que correspondem ao filtro
    /// </summary>
    Task<int> CountAsync(Func<T, bool> predicate);

    /// <summary>
    /// Adiciona uma entidade
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adiciona múltiplas entidades
    /// </summary>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Atualiza uma entidade
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Atualiza múltiplas entidades
    /// </summary>
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Remove uma entidade por ID
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Remove uma entidade
    /// </summary>
    Task<bool> DeleteAsync(T entity);

    /// <summary>
    /// Remove múltiplas entidades
    /// </summary>
    Task<int> DeleteRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Persiste as mudanças no banco de dados
    /// </summary>
    Task<int> SaveChangesAsync();
}
