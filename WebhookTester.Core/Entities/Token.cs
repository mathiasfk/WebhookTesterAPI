namespace WebhookTester.Core.Entities
{
    public class Token
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset LastUsed { get; set; } = DateTimeOffset.UtcNow;
    }
}
