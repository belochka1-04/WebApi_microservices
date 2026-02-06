using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class NModelConfiguration : IEntityTypeConfiguration<NModel>
    {
        public void Configure(EntityTypeBuilder<NModel> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_Model");
            entity.ToTable("Model");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.Property(e => e.model)
                .HasMaxLength(150)
                .HasColumnName("Title")
                .HasDefaultValue(null);

            entity.Property(e => e.WebLink)
                .HasMaxLength(255)
                .HasColumnName("Link")
                .HasDefaultValue(null);

            entity.Property(e => e.CleanedModel)
                .HasMaxLength(255)
                .HasColumnName("cleaned_model");

            entity.Property(e => e.SourceId)
                .IsRequired()
                .HasColumnName("SiteId");

            entity.Property(e => e.Token)
                .HasMaxLength(6)
                .HasColumnName("token")
                .HasDefaultValue(null);
        }
    }
}