using Application.Dtos;
using Application.Interfaces;

namespace Infrastructure.Messaging.InMemory
{
    /// <summary>
    /// Default publisher. Delegates to the in-process <see cref="INotificationQueue"/>
    /// (System.Threading.Channels) that <c>NotificationWorker</c> drains.
    /// Used when Messaging:Provider is "InMemory".
    /// </summary>
    public class InMemoryNotificationPublisher : INotificationPublisher
    {
        private readonly INotificationQueue _queue;

        public InMemoryNotificationPublisher(INotificationQueue queue)
        {
            _queue = queue;
        }

        public async Task PublishAsync(NotificationJob job, CancellationToken cancellationToken = default)
        {
            await _queue.EnqueueAsync(job);
        }
    }
}
