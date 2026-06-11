using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotificationQueue _notificationQueue;

        public MessageService(
            IMessageRepository repository,
            ICategoryRepository categoryRepository,
            INotificationQueue notificationQueue) { 
            _repository = repository;
            _categoryRepository = categoryRepository;
            _notificationQueue = notificationQueue;
        }

        public async Task<Message> CreateAsync(CreateMessageDto createMessage)
        {

            if(!await _categoryRepository.ExistsAsync(createMessage.CategoryId))
                throw new ArgumentException("Invalid category ID.");

            if(string.IsNullOrEmpty(createMessage.Body))
                throw new ArgumentException("Message body cannot be empty.");

            var message = new Message
            {
                Body = createMessage.Body,
                CategoryId = createMessage.CategoryId,
                CorrelationId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(message);

            //dispatchs the message to the notification dispatcher service via queue
            await _notificationQueue.EnqueueAsync(new NotificationJob(message.Id, message.CorrelationId));

            //dispatchs the message to the notification dispatcher service directly, no queue
            //await _dispatcher.DispatchAsync(message);

            return message;
        }
    }
}
