using Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface INotificationJobProcessor
    {
        Task ProcessAsync(NotificationJob job, CancellationToken cancellationToken);
    }
}
