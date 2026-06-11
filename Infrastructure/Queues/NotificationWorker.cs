using Application.Abstractions;
using Application.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Queues
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationQueue _queue;

        public NotificationWorker(
            IServiceProvider serviceProvider,
            INotificationQueue queue)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job =
                    await _queue.DequeueAsync(stoppingToken);

                using var scope =
                    _serviceProvider.CreateScope();

                var dispatcher = scope.ServiceProvider.GetRequiredService<INotificationDispatcherService>();

                var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();

                var message = await messageRepository.GetByIdAsync(job.MessageId);

                await dispatcher.DispatchAsync(message);
            }
        }
    }
}
