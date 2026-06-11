using Application.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface INotificationQueue
    {
        ValueTask EnqueueAsync(NotificationJob job);

        ValueTask<NotificationJob> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
