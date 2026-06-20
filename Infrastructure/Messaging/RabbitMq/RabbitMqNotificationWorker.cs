using System.Text.Json;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging.RabbitMq
{
    /// <summary>
    /// Consumes notification jobs from RabbitMQ and hands each one to
    /// <see cref="INotificationJobProcessor"/> inside its own DI scope.
    /// Successful jobs are acked; failures are nacked without requeue so they
    /// dead-letter (see <see cref="RabbitMqTopology"/>). Replaces NotificationWorker
    /// when Messaging:Provider is "RabbitMq".
    /// </summary>
    public sealed class RabbitMqNotificationWorker : BackgroundService
    {
        private readonly RabbitMqConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqNotificationWorker> _logger;
        private IChannel? _channel;

        public RabbitMqNotificationWorker(
            RabbitMqConnection connection,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqNotificationWorker> logger)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = await _connection.GetConnectionAsync(stoppingToken);
            _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await RabbitMqTopology.DeclareAsync(_channel, _options, stoppingToken);

            // Fair dispatch: don't hand a consumer more than PrefetchCount unacked messages.
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _options.PrefetchCount,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnReceivedAsync;

            await _channel.BasicConsumeAsync(
                queue: _options.Queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "RabbitMQ consumer started on queue {Queue}", _options.Queue);

            // Keep the background service alive until the host shuts down.
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // expected on shutdown
            }
        }

        private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            NotificationJob? job = null;
            try
            {
                job = JsonSerializer.Deserialize<NotificationJob>(eventArgs.Body.Span);

                if (job is null)
                {
                    _logger.LogWarning("Received a message that could not be deserialized; dead-lettering.");
                    await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider
                    .GetRequiredService<INotificationJobProcessor>();

                await processor.ProcessAsync(job, eventArgs.CancellationToken);

                await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process notification job for message {MessageId}; dead-lettering.",
                    job?.MessageId);

                // requeue:false -> routed to the dead-letter exchange rather than redelivered forever.
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            if (_channel is not null)
                await _channel.DisposeAsync();
        }
    }
}
