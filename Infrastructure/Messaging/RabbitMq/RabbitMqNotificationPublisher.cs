using System.Text.Json;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.RabbitMq
{
    /// <summary>
    /// Publishes notification jobs to the RabbitMQ work queue as persistent,
    /// JSON-encoded messages. Used when Messaging:Provider is "RabbitMq".
    /// A single channel is reused; publishes are serialized via a gate because
    /// an <see cref="IChannel"/> is not safe for concurrent use.
    /// </summary>
    public sealed class RabbitMqNotificationPublisher : INotificationPublisher, IAsyncDisposable
    {
        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqOptions _options;
        private readonly SemaphoreSlim _gate = new(1, 1);
        private IChannel? _channel;

        public RabbitMqNotificationPublisher(
            RabbitMqConnection connection,
            IOptions<RabbitMqOptions> options)
        {
            _connection = connection;
            _options = options.Value;
        }

        public async Task PublishAsync(NotificationJob job, CancellationToken cancellationToken = default)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(job);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                CorrelationId = job.CorrelationId.ToString(),
                MessageId = job.MessageId.ToString()
            };

            await _gate.WaitAsync(cancellationToken);
            try
            {
                var channel = await EnsureChannelAsync(cancellationToken);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _options.Queue,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        private async Task<IChannel> EnsureChannelAsync(CancellationToken cancellationToken)
        {
            if (_channel is { IsOpen: true })
                return _channel;

            var connection = await _connection.GetConnectionAsync(cancellationToken);
            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await RabbitMqTopology.DeclareAsync(_channel, _options, cancellationToken);
            return _channel;
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
                await _channel.DisposeAsync();

            _gate.Dispose();
        }
    }
}
