namespace WebhookTesterAPI.DTOs
{
    public record WebhookRequestDTO(
        Guid Id,
        string HttpMethod,
        Dictionary<string, string[]> Headers,
        string Body,
        DateTimeOffset ReceivedAt
    );
}