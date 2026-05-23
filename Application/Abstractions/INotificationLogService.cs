using Application.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{
    public interface INotificationLogService
    {
        Task<List<NotificationLogDto>> GetAllAsync();
    }
}
