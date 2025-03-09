using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface IWebhookService
    {
        Task<OperationResult<Webhook>> CreateWebhook(Guid token);

        Task<OperationResult<IEnumerable<Webhook>>> ListWebhooks(Guid token);

        Task<OperationResult> DeleteWebhook(Guid token, Guid webhookId);

        Task<OperationResult<IEnumerable<WebhookRequest>>> GetRequests(Guid token, Guid webhookId);

        Task<OperationResult> HandleRequestAsync(Guid webhookId, WebhookRequest request);
    }
}
