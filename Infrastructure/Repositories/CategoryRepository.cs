using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public readonly AppDbContext _context;

        public CategoryRepository(AppDbContext appDbContext) {
            _context = appDbContext;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int categoryId)
        {
            return await _context.Categories
                .AnyAsync(x => x.Id == categoryId);
        }
    }
}
