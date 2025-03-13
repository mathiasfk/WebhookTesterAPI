namespace WebhookTester.Core.Interfaces
{
    public interface ICache<T>
    {
        Task SetAsync(string key, T value, TimeSpan? expiration = null);
        Task<T?> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
