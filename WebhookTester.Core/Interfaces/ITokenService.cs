using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface ITokenService
    {
        Task<OperationResult<Token>> CreateToken();
        Task<OperationResult<Token>> ValidateToken(string id);
        Task<OperationResult> RefreshToken(string id);
    }
}
