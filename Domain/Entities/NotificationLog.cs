using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class NotificationLog : BaseEntity
    {
        public int MessageId { get; set; }
        public int ChannelId { get; set; }
        public int UserId { get; set; }
        public int RetryCount { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public NotificationLogStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid CorrelationId { get; set; }
        public virtual Message Message { get; set; }
        public virtual Channel Channel { get; set; }
        public virtual User User { get; set; }

        public static NotificationLog Success(User user, Message message, Channel userChannel)
        {
            return new NotificationLog
            {
                UserId = user?.Id ?? 0,
                MessageId = message?.Id ?? 0,
                ChannelId = userChannel?.Id ?? 0,
                Message = message,
                Channel = userChannel,
                User = user,
                CorrelationId = message?.CorrelationId ?? Guid.Empty,
                DeliveredAt = DateTime.UtcNow,
                ErrorMessage = null
            };
        }

        public static NotificationLog Failure(User user, Message message, Channel userChannel, string errorMessage)
        {
            return new NotificationLog
            {
                UserId = user?.Id ?? 0,
                MessageId = message?.Id ?? 0,
                ChannelId = userChannel?.Id ?? 0,
                Message = message,
                Channel = userChannel,
                User = user,
                CorrelationId = message?.CorrelationId ?? Guid.Empty,
                DeliveredAt = null,
                ErrorMessage = errorMessage
            };
        }
    }
}
