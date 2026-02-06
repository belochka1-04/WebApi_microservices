using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class MaskConfiguration : IEntityTypeConfiguration<Mask>
    {
        public void Configure(EntityTypeBuilder<Mask> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_Masks_id");
            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.mask)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("mask");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");
        }
    }
}