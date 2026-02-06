using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelDocumentConfiguration : IEntityTypeConfiguration<ModelDocument>
    {
        public void Configure(EntityTypeBuilder<ModelDocument> entity)
        {
            entity.ToTable("ModelDocument");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Model)
                .WithMany()
                .HasForeignKey(e => e.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Document)
                .WithMany()
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}