using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class WebhooksService(IWebhooksRepository repository, IServerSentEventsService sse) : IWebhookService
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
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null || webhook.OwnerToken != token)
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

        public async Task<bool> HandleRequestAsync(Guid webhookId, WebhookRequest request)
        {
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null)
                return false;

            await repository.AddRequestAsync(request);
            await sse.WriteToChannelAsync(webhookId, request);

            return true;
        }
    }
}
