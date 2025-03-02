using WebhookTester.Core.Interfaces;

namespace WebhookTester.Core.Services
{
    public class TokenService : ITokenService
    {
        public Guid CreateToken()
        {
            return Guid.NewGuid();
        }
    }
}
