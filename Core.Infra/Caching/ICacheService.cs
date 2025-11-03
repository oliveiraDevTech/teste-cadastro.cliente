namespace Core.Infra.Caching;

/// <summary>
/// Interface para serviço de cache distribuído
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtém um valor do cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <returns>Valor em cache ou null se não encontrado</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Define um valor no cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <param name="value">Valor a armazenar</param>
    /// <param name="expiration">Tempo de expiração do cache</param>
    /// <returns>Task assíncrona</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Remove um valor do cache
    /// </summary>
    /// <param name="key">Chave do cache</param>
    /// <returns>Task assíncrona</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Remove múltiplos valores do cache
    /// </summary>
    /// <param name="keys">Chaves do cache</param>
    /// <returns>Task assíncrona</returns>
    Task RemoveAsync(params string[] keys);

    /// <summary>
    /// Verifica se uma chave existe no cache
    /// </summary>
    /// <param name="key">Chave do cache</param>
    /// <returns>True se existir, false caso contrário</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Limpa todo o cache
    /// </summary>
    /// <returns>Task assíncrona</returns>
    Task ClearAsync();
}
