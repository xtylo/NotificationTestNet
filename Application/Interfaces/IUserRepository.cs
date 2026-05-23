using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetSubscribedUsersAsync(int categoryId);

        Task<User?> GetByIdAsync(int id);
    }
}
