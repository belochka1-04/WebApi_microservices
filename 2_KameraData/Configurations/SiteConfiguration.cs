// SiteConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_Site");
        entity.ToTable("Site");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        entity.Property(e => e.Title)
            .HasMaxLength(50)
            .HasColumnName("Title");

        entity.Property(e => e.Confidence)
            .HasColumnName("Confidence");

        entity.Property(e => e.DataTypes)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("data_types");

        entity.Property(e => e.FolderPath)
            .HasMaxLength(500)
            .HasColumnName("folder_path");

        entity.Property(e => e.LinkTemplate)
            .HasMaxLength(500)
            .HasColumnName("Link_template");
    }
}
