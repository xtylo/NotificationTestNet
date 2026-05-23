using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Channel : BaseEntity
    {
        public string Name { get; set; }
        public NotificationChannelType ChannelType { get; set; }
        public virtual ICollection<User> User { get; set; } = new List<User>();
    }
}
