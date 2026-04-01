namespace GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;

using StackExchange.Redis;

public interface IRedisService
{
    Task SetAsync(string key, string value, TimeSpan expiry);
    Task<string?> GetAsync(string key);
    Task DeleteAsync(string key);
}