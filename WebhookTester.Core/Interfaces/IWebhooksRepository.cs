using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface IWebhooksRepository
    {
        Task<Webhook?> GetByIdAsync(Guid id);

        Task<List<Webhook>> GetByTokenAsync(Guid token);

        Task AddAsync(Webhook webhook);

        Task RemoveAsync(Webhook webhook);

        Task AddRequestAsync(WebhookRequest request);
    }
}
