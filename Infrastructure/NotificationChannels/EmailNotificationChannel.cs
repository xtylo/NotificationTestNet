using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.NotificationChannels
{
    internal class EmailNotificationChannel : INotificationChannel
    {
        public NotificationChannelType ChannelType => NotificationChannelType.Email;

        public Task<NotificationResult> SendAsync(User user, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
