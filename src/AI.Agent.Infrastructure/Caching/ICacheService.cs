namespace AI.Agent.Infrastructure.Caching;

/// <summary>
/// Interface for caching service
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from the cache
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Sets a value in the cache
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from the cache
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Checks if a key exists in the cache
    /// </summary>
    Task<bool> ExistsAsync(string key);
} 