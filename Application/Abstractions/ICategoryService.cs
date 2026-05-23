using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
    }
}
