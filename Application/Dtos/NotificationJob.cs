using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record NotificationJob(
        int MessageId,
        Guid CorrelationId
    );
}
