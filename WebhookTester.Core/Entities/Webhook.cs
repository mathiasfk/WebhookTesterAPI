namespace WebhookTester.Core.Entities
{
    public class Webhook
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OwnerToken { get; set; } = Guid.Empty;
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public List<WebhookRequest> Requests { get; set; } = new();
    }
}
