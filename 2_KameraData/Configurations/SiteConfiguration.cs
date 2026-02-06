using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
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
                .HasColumnName("Title")
                .HasDefaultValue(null);

            entity.Property(e => e.confidence)
                .HasColumnName("Confidence")
                .HasDefaultValue(null);

            entity.Property(e => e.DataTypes)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("datatypes")
                .HasDefaultValue(null);

            entity.Property(e => e.FolderPath)
                .HasMaxLength(500)
                .HasColumnName("folderpath")
                .HasDefaultValue(null);

            entity.Property(e => e.Link_template)
                .HasColumnName("Linktemplate");
        }
    }
}