using WebhookTester.Core.Common;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class TokenService : ITokenService
    {
        public OperationResult<Guid> CreateToken()
        {
            return OperationResult<Guid>.SuccessResult(Guid.NewGuid());
        }
    }
}
