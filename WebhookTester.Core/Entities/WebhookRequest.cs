//using System.ComponentModel.DataAnnotations.Schema;
namespace WebhookTester.Core.Entities
{
    public class WebhookRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WebhookId { get; set; }
        public Webhook Webhook { get; set; } = null!;
        public string HttpMethod { get; set; } = string.Empty;
        public Dictionary<string, string[]> Headers { get; set; } = [];
        public string Body { get; set; } = string.Empty;
        public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
