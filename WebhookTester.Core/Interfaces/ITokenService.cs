using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;

namespace WebhookTester.Core.Interfaces
{
    public interface ITokenService
    {
        Task<OperationResult<Token>> CreateToken();
    }
}
