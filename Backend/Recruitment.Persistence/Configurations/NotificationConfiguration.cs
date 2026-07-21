using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;


namespace Recruitment.Persistence.Configurations
{
    public class NotificationConfiguration :
        IEntityTypeConfiguration<Notification>
    {


        public void Configure(
            EntityTypeBuilder<Notification> builder)
        {

            builder.ToTable("Notifications");


            builder.HasKey(x => x.Id);



            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();



            builder.Property(x => x.Message)
                .HasMaxLength(1000)
                .IsRequired();



            builder.Property(x => x.Type)
                .HasMaxLength(50);



            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
