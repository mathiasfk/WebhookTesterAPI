using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class WebhooksService() : IWebhookService
    {
        public Task<Webhook> CreateWebhook()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Webhook>> ListWebhooks()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteWebhook(Guid webhookId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WebhookRequest>> GetRequests(Guid webhookId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HandleRequestAsync(Guid webhookId, WebhookRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
