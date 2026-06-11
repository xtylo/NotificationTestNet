using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using Channel = System.Threading.Channels.Channel;

namespace Infrastructure.Queues
{
    public class NotificationQueue : INotificationQueue
    {
        private readonly Channel<NotificationJob> _queue;

        public NotificationQueue()
        {
            _queue = Channel.CreateUnbounded<NotificationJob>();
        }

        public async ValueTask EnqueueAsync(NotificationJob job)
        {
            await _queue.Writer.WriteAsync(job);
        }

        public async ValueTask<NotificationJob> DequeueAsync(
            CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(
                cancellationToken);
        }
    }
}
