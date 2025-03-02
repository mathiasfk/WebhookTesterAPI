using Microsoft.EntityFrameworkCore;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.Infrastructure.Repositories
{
    public class WebhooksRepository(WebhookTesterDbContext context) : IWebhooksRepository
    {
        private readonly WebhookTesterDbContext _context = context;


        public async Task<Webhook?> GetByIdAsync(Guid id)
        {
            return await _context.Webhooks
                .Include(w => w.Requests)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Webhook>> GetByTokenAsync(Guid token)
        {
            return await _context.Webhooks
                .Where(w => w.OwnerToken == token)
                .ToListAsync();
        }

        public async Task AddAsync(Webhook webhook)
        {
            _context.Webhooks.Add(webhook);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Webhook webhook)
        {
            _context.Webhooks.Remove(webhook);
            await _context.SaveChangesAsync();
        }

        public async Task AddRequestAsync(WebhookRequest request)
        {
            _context.WebhookRequests.Add(request);
            await _context.SaveChangesAsync();
        }
    }
}
