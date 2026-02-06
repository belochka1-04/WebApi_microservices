using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PartSourceConfiguration : IEntityTypeConfiguration<PartSource>
    {
        public void Configure(EntityTypeBuilder<PartSource> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_parts_sources");
            entity.ToTable("parts_sources");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.SourceName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("source_name");

            entity.Property(e => e.Link)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasColumnName("link");

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("data_type");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("updated_at");

            entity.Property(e => e.Status)
                .HasColumnName("status");
        }
    }
}