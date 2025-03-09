namespace WebhookTester.Core.Common
{
    public record Error()
    {
        public required string Message { get; set; }
        public required ErrorCode Code { get; set; }
    }
}
