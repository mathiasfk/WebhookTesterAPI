using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class TokenService(ITokenRepository repository) : ITokenService
    {
        public async Task<OperationResult<Token>> CreateToken()
        {
            var token = new Token() { Id = Guid.NewGuid(), Created = DateTimeOffset.UtcNow };
            await repository.AddAsync(token);

            return OperationResult<Token>.SuccessResult(token);
        }
    }
}
