using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class GoogleSerpRawConfiguration : IEntityTypeConfiguration<GoogleSerpRaw>
    {
        public void Configure(EntityTypeBuilder<GoogleSerpRaw> builder)
        {
            builder.ToTable("google_serp_raw");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.GoogleModelRequestId)
                .HasColumnName("google_model_request_id")
                .IsRequired();

            builder.Property(x => x.QueryText)
                .HasColumnName("query_text")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.GooglePage)
                .HasColumnName("google_page")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(1000);

            builder.Property(x => x.Snippet)
                .HasColumnName("snippet")
                .HasMaxLength(2000);

            builder.Property(x => x.Url)
                .HasColumnName("url")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();
        }
    }
}
