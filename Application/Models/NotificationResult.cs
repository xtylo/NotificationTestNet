using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class NotificationResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public int RetryCount { get; set; }

        public DateTime ProcessedAt { get; set; }

        public static NotificationResult Successful()
        {
            return new NotificationResult
            {
                Success = true,
                ProcessedAt = DateTime.UtcNow
            };
        }

        public static NotificationResult Failed(
            string errorMessage,
            int retryCount = 0)
        {
            return new NotificationResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                RetryCount = retryCount,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
