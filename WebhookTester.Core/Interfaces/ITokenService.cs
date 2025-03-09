using WebhookTester.Core.Common;

namespace WebhookTester.Core.Interfaces
{
    public interface ITokenService
    {
        public OperationResult<Guid> CreateToken();
    }
}
