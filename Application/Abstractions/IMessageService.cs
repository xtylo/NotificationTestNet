using Application.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions
{
    public interface IMessageService
    {
        Task<Message> CreateAsync(CreateMessageDto message);
    }
}
