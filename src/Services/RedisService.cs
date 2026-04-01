namespace GerenciadorFuncionarios.Services;

using GerenciadorFuncionarios.Adapters;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public Task SetAsync(string key, string value, TimeSpan expiry)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string key)
    {
        throw new NotImplementedException();
    }
}