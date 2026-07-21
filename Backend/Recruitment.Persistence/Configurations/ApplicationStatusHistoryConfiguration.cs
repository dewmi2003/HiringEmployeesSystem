using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
        {
            builder.ToTable("ApplicationStatusHistories");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.OldStatus).HasMaxLength(50).IsRequired();
            builder.Property(a => a.NewStatus).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Comments).HasMaxLength(2000);

            builder.HasOne(a => a.Application)
                .WithMany()
                .HasForeignKey(a => a.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.ChangedByUser)
                .WithMany()
                .HasForeignKey(a => a.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
