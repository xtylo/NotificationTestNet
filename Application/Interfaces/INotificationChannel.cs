using System;
using System.Collections.Generic;
using System.Text;
using Application.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface INotificationChannel
    {
        NotificationChannelType ChannelType { get; }

        Task<NotificationResult> SendAsync(
            User user,
            Message message);
    }
}
