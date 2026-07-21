using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruitment.Domain.Entities;


namespace Recruitment.Persistence.Configurations
{

public class RefreshTokenConfiguration :
IEntityTypeConfiguration<RefreshToken>
{


public void Configure(
EntityTypeBuilder<RefreshToken> builder)
{


builder.ToTable("RefreshTokens");


builder.HasKey(x=>x.Id);



builder.Property(x=>x.Token)
.HasMaxLength(500)
.IsRequired();



builder.HasOne(x=>x.User)
.WithMany()
.HasForeignKey(x=>x.UserId)
.OnDelete(DeleteBehavior.Cascade);



}

}

}