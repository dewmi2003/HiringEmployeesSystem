using Recruitment.Application.DTOs;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;


namespace Recruitment.Application.Services
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationRepository _repository;



        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }



        public async Task CreateAsync(
            NotificationDto dto)
        {

            var notification = new Notification
            {

                Id = Guid.NewGuid(),

                UserId = dto.UserId,

                Title = dto.Title,

                Message = dto.Message,

                Type = dto.Type,

                IsRead = false,

                CreatedAt = DateTime.UtcNow

            };


            await _repository.AddAsync(notification);

        }





        public async Task<IEnumerable<NotificationDto>>
            GetUserNotificationsAsync(Guid userId)
        {

            var data =
                await _repository.GetByUserIdAsync(userId);



            return data.Select(x => new NotificationDto
            {

                Id = x.Id,

                UserId = x.UserId,

                Title = x.Title,

                Message = x.Message,

                Type = x.Type,

                IsRead = x.IsRead,

                CreatedAt = x.CreatedAt

            });

        }





        public async Task MarkAsReadAsync(Guid id)
        {

            await _repository.MarkAsReadAsync(id);

        }





        public async Task<IEnumerable<NotificationDto>>
            GetAllAsync()
        {

            var data =
                await _repository.GetAllAsync();



            return data.Select(x => new NotificationDto
            {

                Id = x.Id,

                UserId = x.UserId,

                Title = x.Title,

                Message = x.Message,

                Type = x.Type,

                IsRead = x.IsRead,

                CreatedAt = x.CreatedAt

            });

        }

    }
}