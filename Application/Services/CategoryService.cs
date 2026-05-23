using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(
            ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
    }
}
