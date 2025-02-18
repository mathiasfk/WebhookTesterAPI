namespace WebhookTesterAPI.DTOs
{
    public record WebhookRequestDTO(
        Guid Id,
        string HttpMethod,
        string Headers,
        string Body,
        DateTimeOffset ReceivedAt
    );
}
