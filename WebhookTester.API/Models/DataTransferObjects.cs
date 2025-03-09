namespace WebhookTester.API.Models
{
    /// <summary>
    /// Contains all Data Transfer Objects (DTOs) used in the API.
    /// </summary>
    public class DataTransferObjects
    {
        /// <summary>
        /// Contains information about a webhook.
        /// </summary>
        /// <param name="Id">Webhook unique identifier</param>
        /// <param name="Url">Webhook URL</param>
        public record WebhookDto(Guid Id, string Url);

        /// <summary>
        /// Represents a request on a webhook.
        /// </summary>
        /// <param name="Id">Request unique identifier</param>
        /// <param name="HttpMethod">Request method</param>
        /// <param name="Headers">Request headers.</param>
        /// <param name="Body">Request body.</param>
        /// <param name="ReceivedAt">Date and time the request was received.</param>
        public record WebhookRequestDto(
            Guid Id,
            string HttpMethod,
            Dictionary<string, string[]> Headers,
            string Body,
            DateTimeOffset ReceivedAt
        );
    }
}
