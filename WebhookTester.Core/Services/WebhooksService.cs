using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class WebhooksService(IWebhooksRepository repository) : IWebhookService
    {
        public async Task<Webhook> CreateWebhook(Guid token)
        {
            var webhook = new Webhook { OwnerToken = token };
            await repository.AddAsync(webhook);

            return webhook;
        }

        public async Task<IEnumerable<Webhook>> ListWebhooks(Guid token)
        {
            return await repository.GetByTokenAsync(token);
        }

        public async Task<bool> DeleteWebhook(Guid token, Guid webhookId)
        {
            var webhook = await repository.GetByIdAsync(token);
            if (webhook == null || webhook.OwnerToken != webhookId)
                return false;

            await repository.RemoveAsync(webhook);
            return true;
        }

        public async Task<IEnumerable<WebhookRequest>> GetRequests(Guid token, Guid webhookId)
        {
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null || webhook.OwnerToken != token)
                return [];

            return webhook.Requests;
        }

        public Task<bool> HandleRequestAsync(Guid webhookId, WebhookRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
