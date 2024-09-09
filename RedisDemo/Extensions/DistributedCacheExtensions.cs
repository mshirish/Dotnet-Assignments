using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisDemo.Extensions;

// This static class contains extension methods to simplify storing and retrieving 
// objects in a distributed cache by serializing and deserializing the objects to/from JSON.
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Stores an object in the distributed cache as a JSON string with optional expiration settings.
    /// </summary>
    /// <typeparam name="T">The type of the object being stored.</typeparam>
    /// <param name="cache">The IDistributedCache instance.</param>
    /// <param name="recordId">The unique key for the cached item.</param>
    /// <param name="data">The object to be cached.</param>
    /// <param name="absoluteExpireTime">Optional. The absolute expiration time after which the item is removed from the cache.</param>
    /// <param name="unusedExpireTime">Optional. The sliding expiration time, where the item is removed after being unused for a specified time.</param>
    /// <returns>An asynchronous task representing the cache storage operation.</returns>
    public static async Task SetRecordAsync<T>(
        this IDistributedCache cache,
        string recordId,
        T data,
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null)
    {
        // Create cache entry options to specify expiration behavior
        var options = new DistributedCacheEntryOptions();

        // Set absolute expiration time to the provided value or default to 60 seconds
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
        // Set sliding expiration time (removes the item if unused for a given duration)
        options.SlidingExpiration = unusedExpireTime;

        // Serialize the object into a JSON string
        var jsonData = JsonSerializer.Serialize(data);

        // Store the JSON string in the cache with the specified recordId and options
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    /// <summary>
    /// Retrieves an object from the distributed cache by its unique key and deserializes it from JSON.
    /// </summary>
    /// <typeparam name="T">The type of the object being retrieved.</typeparam>
    /// <param name="cache">The IDistributedCache instance.</param>
    /// <param name="recordId">The unique key for the cached item.</param>
    /// <returns>The deserialized object if found, or the default value for the type T if not found.</returns>
    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        // Retrieve the cached item as a JSON string by its unique key
        var jsonData = await cache.GetStringAsync(recordId);

        // If no item is found in the cache, return the default value for the type
        if (jsonData is null)
        {
            return default(T);
        }

        // Deserialize the JSON string back into the object and return it
        return JsonSerializer.Deserialize<T>(jsonData);
    }
}
