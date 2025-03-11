using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface ITokenRepository
    {
        Task AddAsync(Token token);
    }
}
