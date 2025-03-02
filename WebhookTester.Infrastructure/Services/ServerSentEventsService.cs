using System.Collections.Concurrent;
using System.Threading.Channels;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Infrastructure.Services
{
    public class ServerSentEventsService : IServerSentEventsService
    {
        private readonly ConcurrentDictionary<Guid, Channel<WebhookRequest>> _channels = new();

        public Channel<WebhookRequest> GetOrCreateChannel(Guid webhookId)
        {
            return _channels.GetOrAdd(webhookId, _ => Channel.CreateUnbounded<WebhookRequest>());
        }

        public async Task WriteToChannelAsync(Guid webhookId, WebhookRequest request)
        {
            if (_channels.TryGetValue(webhookId, out var channel))
            {
                await channel.Writer.WriteAsync(request);
            }
        }
    }
}
