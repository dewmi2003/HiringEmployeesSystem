using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("Candidates");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(20);

            // One-to-One with User
            builder.HasOne(c => c.User)
                .WithOne(u => u.Candidate)
                .HasForeignKey<Candidate>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
