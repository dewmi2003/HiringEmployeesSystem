using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {

        Task AddAsync(Notification notification);


        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);


        Task<Notification?> GetByIdAsync(Guid id);


        Task MarkAsReadAsync(Guid id);


        Task<IEnumerable<Notification>> GetAllAsync();

    }
}