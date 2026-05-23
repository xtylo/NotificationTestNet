using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
    }
}
