using Application.Interfaces;
using Infrastructure.Messaging.InMemory;
using Infrastructure.Messaging.RabbitMq;
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

            //Messaging transport — selected via "Messaging:Provider" (InMemory | RabbitMq).
            //Same INotificationPublisher producer abstraction either way; only the
            //implementation and the consumer hosted-service differ.
            AddMessaging(services, configuration);

            return services;
        }

        private static void AddMessaging(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var provider = configuration["Messaging:Provider"] ?? "InMemory";

            if (provider.Equals("RabbitMq", StringComparison.OrdinalIgnoreCase))
            {
                services.Configure<RabbitMqOptions>(
                    configuration.GetSection("Messaging:RabbitMq"));

                services.AddSingleton<RabbitMqConnection>();
                services.AddSingleton<INotificationPublisher, RabbitMqNotificationPublisher>();
                services.AddHostedService<RabbitMqNotificationWorker>();
            }
            else
            {
                // In-process queue (System.Threading.Channels) drained by NotificationWorker.
                // One queue for all channels; split per channel type later if needed.
                services.AddSingleton<INotificationQueue, NotificationQueue>();
                services.AddSingleton<INotificationPublisher, InMemoryNotificationPublisher>();
                services.AddHostedService<NotificationWorker>();
            }
        }
    }
}
