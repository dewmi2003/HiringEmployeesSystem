using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Data).IsRequired();
        }
    }
}
