using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.ToTable("Resumes");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(r => r.FileName).IsRequired().HasMaxLength(255);
            builder.Property(r => r.FileSize).IsRequired();
            builder.Property(r => r.FileType).IsRequired().HasMaxLength(200);
            builder.Property(r => r.ParsedText).IsRequired();
            builder.Property(r => r.AiScore).IsRequired();
            builder.Property(r => r.Version).IsRequired().HasDefaultValue(1);
            builder.Property(r => r.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(r => r.IsDeleted).IsRequired().HasDefaultValue(false);

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(r => !r.IsDeleted);

            // One-to-Many with Candidate
            builder.HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
