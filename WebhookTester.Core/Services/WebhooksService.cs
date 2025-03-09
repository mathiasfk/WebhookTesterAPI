using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class WebhooksService(IWebhooksRepository repository, IServerSentEventsService sse) : IWebhookService
    {
        public async Task<OperationResult<Webhook>> CreateWebhook(Guid token)
        {
            var webhook = new Webhook { OwnerToken = token };
            await repository.AddAsync(webhook);

            return OperationResult<Webhook>.SuccessResult(webhook);
        }

        public async Task<OperationResult<IEnumerable<Webhook>>> ListWebhooks(Guid token)
        {
            var list = await repository.GetByTokenAsync(token);
            return OperationResult<IEnumerable<Webhook>>.SuccessResult(list);
        }

        public async Task<OperationResult> DeleteWebhook(Guid token, Guid webhookId)
        {
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null || webhook.OwnerToken != token)
                return OperationResult.FailureResult("Webhook not found", ErrorCode.NotFound);

            await repository.RemoveAsync(webhook);
            return OperationResult.SuccessResult();
        }

        public async Task<OperationResult<IEnumerable<WebhookRequest>>> GetRequests(Guid token, Guid webhookId)
        {
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null || webhook.OwnerToken != token)
                return OperationResult<IEnumerable<WebhookRequest>>.FailureResult("Webhook not found", ErrorCode.NotFound);

            return OperationResult<IEnumerable<WebhookRequest>>.SuccessResult(webhook.Requests);
        }

        public async Task<OperationResult> HandleRequestAsync(Guid webhookId, WebhookRequest request)
        {
            var webhook = await repository.GetByIdAsync(webhookId);
            if (webhook == null)
                return OperationResult.FailureResult("Webhook not found", ErrorCode.NotFound);

            await repository.AddRequestAsync(request);
            await sse.WriteToChannelAsync(webhookId, request);

            return OperationResult.SuccessResult();
        }
    }
}
