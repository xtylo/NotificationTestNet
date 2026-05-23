using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Body { get; set; }
        public int CategoryId { get; set; }

        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public virtual Category Category { get; set; }
    }
}
