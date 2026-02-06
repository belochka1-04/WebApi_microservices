using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelTbConfiguration : IEntityTypeConfiguration<ModelTb>
    {
        public void Configure(EntityTypeBuilder<ModelTb> entity)
        {
            entity.ToTable("Model");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.BrandModelId, "IX_Model_BrandModelId");
            entity.HasIndex(e => e.SiteId, "IX_Model_SiteId");
            entity.HasIndex(e => e.CleanedModel, "IX_Model_CleanedModel");
            entity.HasIndex(e => e.Id, "IX_model_id");

            entity.Property(e => e.Title)
                .HasMaxLength(150);

            entity.Property(e => e.Link)
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Token)
                .HasColumnName("token")
                .HasMaxLength(6);

            entity.Property(e => e.CleanedModel)
                .HasColumnName("cleaned_model")
                .HasMaxLength(255);

            entity.Property(e => e.PartCounter)
                .HasColumnName("Part_counter");

            entity.Property(e => e.CpCounter)
                .HasColumnName("CP_counter");

            entity.HasOne(d => d.BrandModel)
                .WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Model_BrandModel");
        }
    }
}