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

        public async Task<NotificationResult> SendAsync(User user, Message message)
        {
            try
            {
                Console.WriteLine(
                    $"Sending email to {user.Email}");

                await Task.Delay(100);

                return NotificationResult.Successful();
            }
            catch (Exception ex)
            {
                return NotificationResult.Failed(ex.Message);
            }
        }
    }
}
