namespace DotNetTutorialGenerator.Infrastructure.Caching
{
    public interface ILLMResponseCache
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }
}
