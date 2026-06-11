using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface INotificationQueue
    {
        ValueTask EnqueueAsync(Message message);

        ValueTask<Message> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
