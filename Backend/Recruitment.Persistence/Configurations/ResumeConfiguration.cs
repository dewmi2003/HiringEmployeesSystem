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
            builder.Property(r => r.ParsedText).IsRequired();
            builder.Property(r => r.AiScore).IsRequired();

            // One-to-Many with Candidate
            builder.HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
