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
        private readonly INotificationPublisher _notificationPublisher;

        public MessageService(
            IMessageRepository repository,
            ICategoryRepository categoryRepository,
            INotificationPublisher notificationPublisher) {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<Message> CreateAsync(CreateMessageDto createMessage)
        {

            if(!await _categoryRepository.ExistsAsync(createMessage.CategoryId))
                throw new ArgumentException("Invalid category ID.");

            if(string.IsNullOrWhiteSpace(createMessage.Body))
                throw new ArgumentException("Message body cannot be empty.");

            var message = new Message
            {
                Body = createMessage.Body,
                CategoryId = createMessage.CategoryId,
                CorrelationId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(message);

            //publishes the job to the configured transport (in-memory / RabbitMQ / ...)
            await _notificationPublisher.PublishAsync(new NotificationJob(message.Id, message.CorrelationId));

            return message;
        }
    }
}
