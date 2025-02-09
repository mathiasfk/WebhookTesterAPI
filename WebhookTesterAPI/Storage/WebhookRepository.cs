using Microsoft.EntityFrameworkCore;
using WebhookTesterAPI.DTOs;
using WebhookTesterAPI.Models;

namespace WebhookTesterAPI.Storage
{
    public class WebhookRepository(WebhookDbContext context)
    {
        private readonly WebhookDbContext _context = context;

        public async Task<Webhook?> GetByIdAsync(Guid id)
        {
            return await _context.Webhooks
                .Include(w => w.Requests)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<WebhookDTO>> GetByTokenAsync(Guid token)
        {
            return await _context.Webhooks
                .Where(w => w.OwnerToken == token)
                .Select(w => new WebhookDTO(w.Id, w.Url))
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
