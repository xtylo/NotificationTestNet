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
        private readonly NotificationDispatcherService _dispatcher;

        public MessageService(
            IMessageRepository repository,
            NotificationDispatcherService dispatcher) { 
            _repository = repository;
            _dispatcher = dispatcher;
        }

        public async Task<Message> CreateAsync(CreateMessageDto createMessage)
        {

            var message = new Message
            {
                Body = createMessage.Body,
                CategoryId = createMessage.CategoryId,
                CorrelationId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(message);

            await _dispatcher.DispatchAsync(message);
            
            return message;
        }
    }
}
