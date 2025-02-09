namespace WebhookTesterAPI.Models
{
    public class Webhook
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Url { get; set; } = string.Empty;
        public Guid OwnerToken { get; set; } = Guid.Empty;
        public List<WebhookRequest> Requests { get; set; } = new();
    }

}
