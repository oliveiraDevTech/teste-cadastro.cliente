namespace Core.Infra.Caching;

/// <summary>
/// Implementação de cache em memória
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public InMemoryCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
                return null;

            return System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration;
            else
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            await _cache.SetStringAsync(key, json, options);
        }
        catch { }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch { }
    }

    public async Task RemoveAsync(params string[] keys)
    {
        try
        {
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }
        }
        catch { }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch
        {
            return false;
        }
    }

    public async Task ClearAsync()
    {
        // Nota: MemoryDistributedCache não suporta limpeza completa
        // Esta é uma limitação do cache em memória
        await Task.CompletedTask;
    }
}
