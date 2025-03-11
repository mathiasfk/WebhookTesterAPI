using Microsoft.EntityFrameworkCore;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Infrastructure.Repositories
{
    public class TokenRepository(WebhookTesterDbContext context) : ITokenRepository
    {
        public async Task AddAsync(Token token)
        {
            context.Tokens.Add(token);
            await context.SaveChangesAsync();
        }
    }
}
