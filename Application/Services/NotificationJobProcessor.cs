using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class NotificationJobProcessor : INotificationJobProcessor
    {
        private readonly IMessageRepository _messageRepository;
        private readonly INotificationDispatcherService _dispatcher;

        public NotificationJobProcessor(
            IMessageRepository messageRepository,
            INotificationDispatcherService dispatcher)
        {
            _messageRepository = messageRepository;
            _dispatcher = dispatcher;
        }

        public async Task ProcessAsync(NotificationJob job, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(job.MessageId);

            if (message is null)
            {
                // TODO: log a warning - job referenced a message that no longer exists
                return;
            }

            await _dispatcher.DispatchAsync(message);
        }
    }
}
