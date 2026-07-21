using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
    {
        public void Configure(EntityTypeBuilder<Evaluation> builder)
        {
            builder.ToTable("Evaluations");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Notes).HasMaxLength(2000);
            builder.Property(e => e.Recommendation).HasMaxLength(50);
            builder.Property(e => e.OverallScore).HasColumnType("float");

            // One-to-Many with Interview (existing)
            builder.HasOne(e => e.Interview)
                .WithMany(i => i.Evaluations)
                .HasForeignKey(e => e.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Recruiter (existing)
            builder.HasOne(e => e.Interviewer)
                .WithMany(r => r.Evaluations)
                .HasForeignKey(e => e.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-One with Application
            builder.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Many-to-One with Candidate
            builder.HasOne(e => e.Candidate)
                .WithMany()
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.NoAction);

            // Many-to-One with HiringManager (User - optional)
            builder.HasOne(e => e.HiringManager)
                .WithMany()
                .HasForeignKey(e => e.HiringManagerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
