using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    internal class NotificationLogService : INotificationLogService
    {
        private readonly INotificationLogRepository _repository;

        public NotificationLogService(INotificationLogRepository repository) { 
            _repository = repository;
        }

        public async Task<List<NotificationLogDto>> GetAllAsync()
        {
            var logs = await _repository.GetAllAsync();

            return logs.Select(log => new NotificationLogDto
            {
                Id = log.Id,
                Message = log.Message.Body,
                MessageCategory = log.Message.Category.Name,
                UserName = log.User.Name,
                DeliveredAt = log.DeliveredAt,
                Status = log.Status,
                ErrorMessage = log.ErrorMessage,
                CorrelationId = log.CorrelationId,
                ChannelName = log.Channel.Name
            }).ToList();
        }
    }
}
