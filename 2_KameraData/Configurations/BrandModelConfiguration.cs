using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class BrandModelConfiguration : IEntityTypeConfiguration<BrandModel>
    {
        public void Configure(EntityTypeBuilder<BrandModel> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_BrandModel");
            entity.ToTable("BrandModel");

            entity.HasIndex(e => e.BrandId, "IX_BrandId");
            entity.HasIndex(e => e.SiteId, "IX_SiteId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.Property(e => e.Code)
                .HasMaxLength(150)
                .HasColumnName("Code")
                .HasDefaultValue(null);

            entity.Property(e => e.Cnt)
                .HasColumnName("Cnt")
                .HasDefaultValue(0);

            entity.Property(e => e.BrandId)
                .HasColumnName("BrandId")
                .HasDefaultValue(null);

            entity.Property(e => e.SiteId)
                .HasColumnName("SiteId")
                .HasDefaultValue(null);

            entity.HasOne<Site>()
                .WithMany()
                .HasForeignKey(e => e.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Brand)
                .WithMany(p => p.BrandModels)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BrandModel_Brand");
        }
    }
}