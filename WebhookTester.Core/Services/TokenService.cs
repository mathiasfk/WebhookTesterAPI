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

        public async Task<OperationResult<Token>> ValidateToken(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guid))
            {
                return OperationResult<Token>.FailureResult("Invalid token", ErrorCode.BadRequest);
            }

            var token = await repository.GetByIdAsync(guid);
            if(token is null)
            {
                return OperationResult<Token>.FailureResult("Invalid token", ErrorCode.Unauthorized);
            }
            if (token.Created < DateTimeOffset.Now.AddDays(-30))
            {
                return OperationResult<Token>.FailureResult("Expired token", ErrorCode.Unauthorized);
            }

            return OperationResult<Token>.SuccessResult(token);
        }
    }
}
