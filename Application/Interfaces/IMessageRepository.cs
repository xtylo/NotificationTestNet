using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(int id);
        Task<Message> CreateAsync(Message message);
    }
}
