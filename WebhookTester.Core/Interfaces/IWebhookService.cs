using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface IWebhookService
    {
        Task<Webhook> CreateWebhook();

        Task<IEnumerable<Webhook>> ListWebhooks();

        Task<bool> DeleteWebhook(Guid webhookId);

        Task<IEnumerable<WebhookRequest>> GetRequests(Guid webhookId);

        Task<bool> HandleRequestAsync(Guid webhookId, WebhookRequest request);
    }
}
