using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record NotificationLogDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageCategory { get; set; }
        public string UserName { get; set; }

        public DateTime? DeliveredAt { get; set; }
        public NotificationLogStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid CorrelationId { get; set; }

        public string ChannelName { get; set; }
    }
}
