using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class SourcesConfiguration : IEntityTypeConfiguration<Sources>
    {
        public void Configure(EntityTypeBuilder<Sources> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_sources_id");
            entity.ToTable("sources");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.SourceName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("source_name");

            entity.Property(e => e.Confidence)
                .IsRequired()
                .HasMaxLength(1)
                .HasColumnName("confidence");

            entity.Property(e => e.DataTypes)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("datatypes")
                .HasDefaultValue(null);

            entity.Property(e => e.FolderPath)
                .HasMaxLength(500)
                .HasColumnName("folder_path")
                .HasDefaultValue(null);
        }
    }
}