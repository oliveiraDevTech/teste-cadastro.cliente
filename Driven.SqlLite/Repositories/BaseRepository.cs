using Microsoft.EntityFrameworkCore;
using Core.Domain.Common;
using Core.Application.Interfaces.Repositories;
using Driven.SqlLite.Data;

namespace Driven.SqlLite.Repositories;

/// <summary>
/// Repositório base genérico com implementação padrão de CRUD
/// Fornece operações comuns para qualquer entidade
/// </summary>
/// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Contexto do banco de dados
    /// </summary>
    protected readonly ApplicationDbContext Context;

    /// <summary>
    /// DbSet da entidade
    /// </summary>
    protected readonly DbSet<T> DbSet;

    /// <summary>
    /// Construtor do repositório base
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    protected BaseRepository(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtém uma entidade por seu ID
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Lista todas as entidades
    /// </summary>
    public virtual IQueryable<T> GetAll()
    {
        return DbSet.AsNoTracking();
    }

    /// <summary>
    /// Obtém todas as entidades ativas
    /// </summary>
    public virtual IQueryable<T> GetAllActive()
    {
        return DbSet.Where(x => x.Ativo).AsNoTracking();
    }

    /// <summary>
    /// Adiciona uma entidade ao repositório
    /// </summary>
    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await DbSet.AddAsync(entity);
    }

    /// <summary>
    /// Adiciona várias entidades ao repositório
    /// </summary>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await DbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    public virtual void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.MarcarComoAtualizada();
        DbSet.Update(entity);
    }

    /// <summary>
    /// Atualiza várias entidades
    /// </summary>
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            entity.MarcarComoAtualizada();
        }

        DbSet.UpdateRange(entities);
    }

    /// <summary>
    /// Remove uma entidade do repositório (exclusão física)
    /// </summary>
    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        DbSet.Remove(entity);
        return true;
    }

    /// <summary>
    /// Remove uma entidade específica
    /// </summary>
    public virtual void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);
    }

    /// <summary>
    /// Remove várias entidades
    /// </summary>
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        DbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Desativa uma entidade (exclusão lógica)
    /// </summary>
    public virtual async Task<bool> DeactivateAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        entity.Desativar();
        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Ativa uma entidade
    /// </summary>
    public virtual async Task<bool> ActivateAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        entity.Ativar();
        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Verifica se uma entidade existe
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Obtém a quantidade de entidades
    /// </summary>
    public virtual async Task<int> CountAsync()
    {
        return await DbSet.CountAsync();
    }

    /// <summary>
    /// Obtém a quantidade de entidades ativas
    /// </summary>
    public virtual async Task<int> CountActiveAsync()
    {
        return await DbSet.Where(x => x.Ativo).CountAsync();
    }

    /// <summary>
    /// Salva todas as alterações no banco de dados
    /// </summary>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }
}
