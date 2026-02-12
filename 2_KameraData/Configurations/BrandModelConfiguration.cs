using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            .HasColumnName("Code");

        entity.Property(e => e.Cnt)
            .HasColumnName("Cnt");

        entity.Property(e => e.BrandId)
            .HasColumnName("BrandId");

        entity.Property(e => e.SiteId)
            .HasColumnName("SiteId");

        entity.HasOne(e => e.Site)
            .WithMany(s => s.BrandModels)
            .HasForeignKey(e => e.SiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_BrandModel_Site");

        entity.HasOne(e => e.Brand)
            .WithMany(b => b.BrandModels)
            .HasForeignKey(e => e.BrandId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_BrandModel_Brand");
    }
}
