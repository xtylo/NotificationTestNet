using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Seeders
{
    internal class ChannelSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Channels.Any())
                return;

            context.Channels.AddRange(
                new Channel { Id = 1, Name = "SMS", ChannelType = Domain.Enums.NotificationChannelType.Sms },
                new Channel { Id = 2, Name = "Email", ChannelType = Domain.Enums.NotificationChannelType.Email },
                new Channel { Id = 3, Name = "Push", ChannelType = Domain.Enums.NotificationChannelType.Push  }
            );

            context.SaveChanges();
        }
    }
}
