using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Recruitment.Domain.Entities.Application>
    {
        public void Configure(EntityTypeBuilder<Recruitment.Domain.Entities.Application> builder)
        {
            builder.ToTable("Applications");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Status).IsRequired().HasMaxLength(50);

            // One-to-Many with Job
            builder.HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Candidate
            builder.HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
