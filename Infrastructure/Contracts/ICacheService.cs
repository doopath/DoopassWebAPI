namespace Infrastructure.Contracts;

public interface ICacheService
{
    public Task Add(string key, string value);
    public Task Delete(string key);
    public Task<string?> Get(string key);
}