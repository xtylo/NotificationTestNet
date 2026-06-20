using Application.Dtos;

namespace Application.Interfaces
{
    /// <summary>
    /// Producer-side abstraction for publishing notification jobs to the
    /// underlying transport (in-memory channel, RabbitMQ, Azure Service Bus, ...).
    /// The consumer side is transport-specific and lives in Infrastructure.
    /// </summary>
    public interface INotificationPublisher
    {
        Task PublishAsync(NotificationJob job, CancellationToken cancellationToken = default);
    }
}
