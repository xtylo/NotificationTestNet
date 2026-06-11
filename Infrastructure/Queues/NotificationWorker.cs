using Application.Dtos;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Queues
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationQueue _queue;
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(
            IServiceProvider serviceProvider,
            INotificationQueue queue,
            ILogger<NotificationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                NotificationJob job;
                try
                {
                    job = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                using var scope = _serviceProvider.CreateScope();

                try
                {
                    var processor = scope.ServiceProvider
                        .GetRequiredService<INotificationJobProcessor>();

                    await processor.ProcessAsync(job, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to process notification job for message {MessageId}",
                        job.MessageId);
                }
            }
        }
    }
}