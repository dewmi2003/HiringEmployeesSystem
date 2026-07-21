using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;


namespace Recruitment.Persistence.Repositories
{
    public class NotificationRepository :
        INotificationRepository
    {

        private readonly ApplicationDbContext _context;



        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task AddAsync(
            Notification notification)
        {

            await _context.Notifications.AddAsync(notification);

            await _context.SaveChangesAsync();

        }




        public async Task<IEnumerable<Notification>>
            GetByUserIdAsync(Guid userId)
        {

            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        }





        public async Task<Notification?> GetByIdAsync(
            Guid id)
        {

            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);

        }





        public async Task MarkAsReadAsync(Guid id)
        {

            var notification =
                await GetByIdAsync(id);



            if(notification != null)
            {

                notification.IsRead = true;

                notification.ReadAt = DateTime.UtcNow;


                await _context.SaveChangesAsync();

            }

        }





        public async Task<IEnumerable<Notification>>
            GetAllAsync()
        {

            return await _context.Notifications
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        }

    }
}