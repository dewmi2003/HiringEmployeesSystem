using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;


namespace Recruitment.Persistence.Configurations
{

public class AuditLogConfiguration 
:IEntityTypeConfiguration<AuditLog>
{


public void Configure(
EntityTypeBuilder<AuditLog> builder)
{

builder.ToTable("AuditLogs");


builder.HasKey(x=>x.Id);



builder.Property(x=>x.Action)
.HasMaxLength(200)
.IsRequired();



builder.Property(x=>x.EntityName)
.HasMaxLength(100);



builder.Property(x=>x.IpAddress)
.HasMaxLength(50);


builder.HasOne(x=>x.User)
.WithMany()
.HasForeignKey(x=>x.UserId)
.OnDelete(DeleteBehavior.SetNull);

}

}

}