using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ReplacesArchiveConfiguration : IEntityTypeConfiguration<ReplacesArchive>
    {
        public void Configure(EntityTypeBuilder<ReplacesArchive> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_replaces_archive");
            entity.ToTable("replaces_archive");

            entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
            entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.PartsAndReplacesId)
                .HasColumnType("int")
                .HasColumnName("parts_and_replaces_id")
                .IsRequired();

            entity.Property(e => e.ReplaceNumber)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("replace_number");

            entity.Property(e => e.PartsSourcesId)
                .HasColumnType("int")
                .HasColumnName("parts_sources_id")
                .IsRequired();

            entity.Property(e => e.AttemptCounter)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .HasColumnName("attempt_counter");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("updated_at");
        }
    }
}