using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class RecruiterConfiguration : IEntityTypeConfiguration<Recruiter>
    {
        public void Configure(EntityTypeBuilder<Recruiter> builder)
        {
            builder.ToTable("Recruiters");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.LastName).IsRequired().HasMaxLength(100);

            // One-to-One with User
            builder.HasOne(r => r.User)
                .WithOne(u => u.Recruiter)
                .HasForeignKey<Recruiter>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Company
            builder.HasOne(r => r.Company)
                .WithMany(c => c.Recruiters)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
