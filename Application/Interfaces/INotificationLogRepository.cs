using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface INotificationLogRepository
    {
        Task CreateAsync(NotificationLog log);

        Task<List<NotificationLog>> GetAllAsync();
    }
}
