using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(j => j.Department)
       .HasMaxLength(100);


            builder.Property(j => j.PostedDate)
       .IsRequired();
            builder.Property(j => j.Description)
                .IsRequired();

            builder.Property(j => j.Requirements)
                .IsRequired();

            builder.Property(j => j.Salary)
                .HasPrecision(18, 2);

            // One-to-Many with Company
            builder.HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Recruiter
            builder.HasOne(j => j.Recruiter)
                .WithMany(r => r.Jobs)
                .HasForeignKey(j => j.RecruiterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}