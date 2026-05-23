using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{
    public interface INotificationDispatcherService
    {
        Task DispatchAsync(Message message);
    }
}
