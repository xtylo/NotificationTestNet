using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetSubscribedUsersAsync(
            int categoryId)
        {
            return await _context.Users
                .Include(x => x.Categories)
                .Include(x => x.Channels)
                .Where(x =>
                    x.Categories.Any(c => c.Id == categoryId))
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(x => x.Channels)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
