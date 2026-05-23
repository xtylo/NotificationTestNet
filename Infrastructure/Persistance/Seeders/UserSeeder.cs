using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Seeders
{
    public static class UserSeeder
    {
        public static void Seed(AppDbContext context)
        {   
            if(context.Users.Any())
                return;

            var emailChannel = context.Channels
                .First(x => x.Id ==
                (int)NotificationChannelType.Email);

            var smsChannel = context.Channels
                .First(x => x.Id ==
                    (int)NotificationChannelType.Sms);

            var pushChannel = context.Channels
                .First(x => x.Id ==
                    (int)NotificationChannelType.Push);

            var sportsCategory = context.Categories
                .First(x => x.Name == "Sports");

            var financeCategory = context.Categories
                .First(x => x.Name == "Finance");

            var moviesCategory = context.Categories
                .First(x => x.Name == "Movies");

            context.Users.AddRange(
                new User { 
                    Id = 1, 
                    Name = "Eduardo Mexia", 
                    Email = "eduardo.mexia@example.com",
                    Phone = "+1234567890",
                    Channels = new List<Channel>
                    {   
                        emailChannel, smsChannel, pushChannel,
                    },
                    Categories = new List<Category>
                    {
                        moviesCategory, sportsCategory
                    }
                    
                },
                new User { 
                    Id = 2, 
                    Name = "Karina Castillo", 
                    Email = "karina.castillo@example.com",
                    Phone = "+1234567891",
                    Channels = new List<Channel> {
                        smsChannel
                    },
                    Categories = new List<Category>
                    {
                        financeCategory, moviesCategory
                    }
                    
                }
            );

            context.SaveChanges();
        }
    }
}
