using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using SQLitePCL;

namespace Infrastructure.Repositories
{
    internal class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<Message> CreateAsync(Message message)
        {
            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();

            return message;
        }
    }
}
