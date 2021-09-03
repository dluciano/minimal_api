using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MinimalApi.DataAccess.Context.Models
{
    record Post(Guid PostId, string Title, string? Content, string CreatedBySub, DateTimeOffset CreatedOn, byte[] RowVersion)
    {
        public class PostEntityConf : IEntityTypeConfiguration<Post>
        {
            public void Configure(EntityTypeBuilder<Post> builder)
            {
                builder.ToTable("Posts");
                builder.HasKey(p => p.PostId);
                builder.Property(p => p.Title)
                    .HasMaxLength(512);
                builder.Property(p => p.CreatedOn)
                    .HasConversion(
                        entityValue => entityValue.ToUniversalTime(),
                        dbValue => dbValue.ToUniversalTime());
                builder.Property(p => p.CreatedBySub).HasMaxLength(255);
                builder.Property(p => p.RowVersion).IsRowVersion();
            }
        }
    }
}
