using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebhookTesterAPI.Models
{
    public class WebhookRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WebhookId { get; set; }
        public Webhook Webhook { get; set; } = null!;
        public string HttpMethod { get; set; } = string.Empty;
        public string HeadersJson { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;

        [NotMapped]
        public Dictionary<string, string[]> Headers
        {
            get => JsonSerializer.Deserialize<Dictionary<string, string[]>>(HeadersJson) ?? new Dictionary<string, string[]>();
            set => HeadersJson = JsonSerializer.Serialize(value);
        }
    }
}