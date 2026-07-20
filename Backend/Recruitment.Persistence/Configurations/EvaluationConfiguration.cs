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
            builder.Property(e => e.Notes).HasMaxLength(1000);

            // One-to-Many with Interview
            builder.HasOne(e => e.Interview)
                .WithMany(i => i.Evaluations)
                .HasForeignKey(e => e.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Recruiter
            builder.HasOne(e => e.Interviewer)
                .WithMany(r => r.Evaluations)
                .HasForeignKey(e => e.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
