namespace Infrastructure.Messaging.RabbitMq
{
    /// <summary>
    /// Bound from the "Messaging:RabbitMq" configuration section.
    /// </summary>
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";

        /// <summary>Main work queue notification jobs are published to / consumed from.</summary>
        public string Queue { get; set; } = "notifications";

        /// <summary>Dead-letter exchange that rejected (poison) messages are routed to.</summary>
        public string DeadLetterExchange { get; set; } = "notifications.dlx";

        /// <summary>Queue bound to the dead-letter exchange where poison messages land.</summary>
        public string DeadLetterQueue { get; set; } = "notifications.dlq";

        /// <summary>Max unacknowledged messages delivered to a single consumer at a time.</summary>
        public ushort PrefetchCount { get; set; } = 10;
    }
}
