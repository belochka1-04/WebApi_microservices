using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelPartConfiguration : IEntityTypeConfiguration<ModelPart>
    {
        public void Configure(EntityTypeBuilder<ModelPart> entity)
        {
            entity.ToTable("ModelPart");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ModelId)
                .IsRequired();

            entity.Property(e => e.PartId)
                .IsRequired();

            entity.HasOne(e => e.Model)
                .WithMany()
                .HasForeignKey(e => e.ModelId);

            entity.HasOne(e => e.Part)
                .WithMany()
                .HasForeignKey(e => e.PartId);
        }
    }
}