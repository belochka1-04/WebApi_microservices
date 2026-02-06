using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PartsConfiguration : IEntityTypeConfiguration<Parts>
    {
        public void Configure(EntityTypeBuilder<Parts> entity)
        {
            entity.ToTable("Part");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PartNumber)
                .HasColumnName("Title")
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.Link)
                .HasColumnName("Link")
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.PartName)
                .HasColumnName("Description")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.Property(e => e.Note)
                .HasColumnName("Note")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.Property(e => e.SiteId)
                .HasColumnName("SiteId");

            entity.HasOne(d => d.Site)
                .WithMany()
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}