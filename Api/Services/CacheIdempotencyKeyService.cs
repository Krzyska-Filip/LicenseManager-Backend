using Microsoft.Extensions.Caching.Memory;

namespace Api.Services;

public class CacheIdempotencyKeyService : IIdempotencyKeyService
{
    private IMemoryCache _cache;
    
    public CacheIdempotencyKeyService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public IdempotencyBody? Get(string idempotencyKey)
    {
        if (_cache.TryGetValue(idempotencyKey, out object? value) &&
            value is IdempotencyBody key)
        {
            return key;
        }

        return null;
    }

    public bool Set(string idempotencyKey, IdempotencyBody idempotencyBody)
    {
        _cache.Set(idempotencyKey, idempotencyBody, TimeSpan.FromMinutes(10));
        return true;
    }
}