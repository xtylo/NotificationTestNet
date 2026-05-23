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
        private readonly INotificationDispatcherService _dispatcher;
        private readonly ICategoryRepository _categoryRepository;

        public MessageService(
            IMessageRepository repository,
            INotificationDispatcherService dispatcher,
            ICategoryRepository categoryRepository) { 
            _repository = repository;
            _dispatcher = dispatcher;
            _categoryRepository = categoryRepository;
        }

        public async Task<Message> CreateAsync(CreateMessageDto createMessage)
        {

            if(!await _categoryRepository.ExistsAsync(createMessage.CategoryId))
                throw new ArgumentException("Invalid category ID.");

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
