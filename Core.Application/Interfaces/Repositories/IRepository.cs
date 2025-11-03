using Core.Domain.Common;

namespace Core.Application.Interfaces.Repositories;

/// <summary>
/// Interface genérica de repositório
/// Define operações CRUD básicas para qualquer entidade
/// </summary>
/// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtém uma entidade por seu ID
    /// </summary>
    /// <param name="id">ID da entidade</param>
    /// <returns>Entidade encontrada ou null</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lista todas as entidades
    /// </summary>
    /// <returns>IQueryable de entidades para composição de queries</returns>
    IQueryable<T> GetAll();

    /// <summary>
    /// Obtém todas as entidades ativas
    /// </summary>
    /// <returns>IQueryable de entidades ativas</returns>
    IQueryable<T> GetAllActive();

    /// <summary>
    /// Adiciona uma entidade ao repositório
    /// </summary>
    /// <param name="entity">Entidade a adicionar</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Adiciona várias entidades ao repositório
    /// </summary>
    /// <param name="entities">Coleção de entidades</param>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    /// <param name="entity">Entidade com dados atualizados</param>
    void Update(T entity);

    /// <summary>
    /// Atualiza várias entidades
    /// </summary>
    /// <param name="entities">Coleção de entidades</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Remove uma entidade do repositório (exclusão física)
    /// </summary>
    /// <param name="id">ID da entidade a remover</param>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Remove uma entidade específica
    /// </summary>
    /// <param name="entity">Entidade a remover</param>
    void Delete(T entity);

    /// <summary>
    /// Remove várias entidades
    /// </summary>
    /// <param name="entities">Coleção de entidades</param>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Desativa uma entidade (exclusão lógica)
    /// </summary>
    /// <param name="id">ID da entidade</param>
    Task<bool> DeactivateAsync(Guid id);

    /// <summary>
    /// Ativa uma entidade
    /// </summary>
    /// <param name="id">ID da entidade</param>
    Task<bool> ActivateAsync(Guid id);

    /// <summary>
    /// Verifica se uma entidade existe
    /// </summary>
    /// <param name="id">ID da entidade</param>
    /// <returns>True se existe, False caso contrário</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Obtém a quantidade de entidades
    /// </summary>
    /// <returns>Total de entidades</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Obtém a quantidade de entidades ativas
    /// </summary>
    /// <returns>Total de entidades ativas</returns>
    Task<int> CountActiveAsync();

    /// <summary>
    /// Salva todas as alterações no banco de dados
    /// </summary>
    Task<int> SaveChangesAsync();
}
