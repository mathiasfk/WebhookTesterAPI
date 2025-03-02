using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface IWebhookService
    {
        Task<Webhook> CreateWebhook(Guid token);

        Task<IEnumerable<Webhook>> ListWebhooks(Guid token);

        Task<bool> DeleteWebhook(Guid token, Guid webhookId);

        Task<IEnumerable<WebhookRequest>> GetRequests(Guid token, Guid webhookId);

        Task<bool> HandleRequestAsync(Guid webhookId, WebhookRequest request);
    }
}
