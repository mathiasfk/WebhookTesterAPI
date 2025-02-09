namespace WebhookTesterAPI.Models
{
    public class Webhook
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Url { get; set; } = string.Empty;
        public string OwnerToken { get; set; } = string.Empty;
        public List<WebhookRequest> Requests { get; set; } = new();
    }

}
