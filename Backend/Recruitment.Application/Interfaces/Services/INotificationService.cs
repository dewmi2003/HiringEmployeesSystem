using Recruitment.Application.DTOs;

namespace Recruitment.Application.Interfaces.Services
{
    public interface INotificationService
    {

        Task CreateAsync(NotificationDto dto);


        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(
            Guid userId);



        Task MarkAsReadAsync(Guid id);



        Task<IEnumerable<NotificationDto>> GetAllAsync();

    }
}