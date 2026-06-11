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
        private readonly Channel<Message> _queue;

        public NotificationQueue()
        {
            _queue = Channel.CreateUnbounded<Message>();
        }

        public async ValueTask EnqueueAsync(Message message)
        {
            await _queue.Writer.WriteAsync(message);
        }

        public async ValueTask<Message> DequeueAsync(
            CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(
                cancellationToken);
        }
    }
}
