using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly AppDbContext _context;

        public NotificationLogRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(NotificationLog log)
        {
            await _context.NotificationLogs.AddAsync(log);

            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationLog>> GetAllAsync()
        {
            return await _context.NotificationLogs
                .Include(x => x.User)
                .Include(x => x.Message)
                    .ThenInclude(m => m.Category)
                .Include(x => x.Channel)
                .OrderByDescending(x => x.DeliveredAt)
                .ToListAsync();
        }

    }
}
