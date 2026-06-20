using RabbitMQ.Client;

namespace Infrastructure.Messaging.RabbitMq
{
    /// <summary>
    /// Declares the broker topology. All declarations are idempotent, so both the
    /// publisher and the consumer can safely call this when they open a channel.
    ///
    /// Layout: a durable work queue with a dead-letter exchange. Messages that are
    /// rejected (BasicNack with requeue:false) are routed to the DLX and land in the
    /// dead-letter queue for inspection instead of being lost or redelivered forever.
    /// </summary>
    internal static class RabbitMqTopology
    {
        public static async Task DeclareAsync(
            IChannel channel,
            RabbitMqOptions options,
            CancellationToken cancellationToken = default)
        {
            // Dead-letter exchange + queue
            await channel.ExchangeDeclareAsync(
                exchange: options.DeadLetterExchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: options.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: options.DeadLetterQueue,
                exchange: options.DeadLetterExchange,
                routingKey: string.Empty,
                cancellationToken: cancellationToken);

            // Main work queue, dead-lettering to the exchange above
            var args = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = options.DeadLetterExchange
            };

            await channel.QueueDeclareAsync(
                queue: options.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args,
                cancellationToken: cancellationToken);
        }
    }
}
