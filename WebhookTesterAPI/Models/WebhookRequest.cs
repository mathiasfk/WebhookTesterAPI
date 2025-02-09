namespace WebhookTesterAPI.Models
{
    public class WebhookRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WebhookId { get; set; }
        public Webhook Webhook { get; set; } = null!;
        public string HttpMethod { get; set; } = string.Empty;
        public string Headers { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }

}
