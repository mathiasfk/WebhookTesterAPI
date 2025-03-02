using System.Threading.Channels;
using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface IServerSentEventsService
    {
        Channel<WebhookRequest> GetOrCreateChannel(Guid webhookId);
        Task WriteToChannelAsync(Guid webhookId, WebhookRequest request);
    }
}
