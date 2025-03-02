namespace WebhookTester.API.Models
{
    public class DataTransferObjects
    {
        public record WebhookDTO(Guid Id, string Url);
        public record WebhookRequestDTO(
            Guid Id,
            string HttpMethod,
            Dictionary<string, string[]> Headers,
            string Body,
            DateTimeOffset ReceivedAt
        );
    }
}
