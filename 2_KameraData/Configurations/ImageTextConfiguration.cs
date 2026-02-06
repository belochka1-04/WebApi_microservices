using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ImageTextConfiguration : IEntityTypeConfiguration<ImageText>
    {
        public void Configure(EntityTypeBuilder<ImageText> entity)
        {
            entity.ToTable("image_text");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.text)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("text");

            entity.Property(e => e.file)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("file");

            entity.Property(e => e.folder)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("folder");
        }
    }
}