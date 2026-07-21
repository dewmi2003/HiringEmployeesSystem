using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class HiringDecisionConfiguration : IEntityTypeConfiguration<HiringDecision>
    {
        public void Configure(EntityTypeBuilder<HiringDecision> builder)
        {
            builder.ToTable("HiringDecisions");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Decision).HasMaxLength(50).IsRequired();
            builder.Property(h => h.Comments).HasMaxLength(2000);

            builder.HasOne(h => h.Application)
                .WithMany()
                .HasForeignKey(h => h.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.DecidedByUser)
                .WithMany()
                .HasForeignKey(h => h.DecidedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
