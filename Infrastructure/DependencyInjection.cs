using Application.Interfaces;
using Infrastructure.NotificationChannels;
using Infrastructure.Persistance;
using Infrastructure.Queues;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString(
                        "DefaultConnection")));

            // Repositories
            services.AddScoped<
                IUserRepository,
                UserRepository>();

            services.AddScoped<
                ICategoryRepository,
                CategoryRepository>();

            services.AddScoped<
                IMessageRepository,
                MessageRepository>();

            services.AddScoped<
                INotificationLogRepository,
                NotificationLogRepository>();

            //Channels
            services.AddScoped<
                INotificationChannel,
                EmailNotificationChannel>();

            services.AddScoped<
                INotificationChannel,
                SMSNotificationChannel>();

            services.AddScoped<
                INotificationChannel,
                PushNotificationChannel>();

            //Queues
            //One queue for all channels, if needed we can have separate queues for each channel type
            services.AddSingleton<
                INotificationQueue,
                NotificationQueue>();

            services.AddHostedService<NotificationWorker>();

            return services;
        }
    }
}
