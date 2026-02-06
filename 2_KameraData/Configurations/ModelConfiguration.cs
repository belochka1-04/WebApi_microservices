using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelConfiguration : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_models_id");
            entity.ToView("models");

            entity.HasIndex(e => e.CategoriesId, "IX_CategoriesId");
            entity.HasIndex(e => e.model, "IX_Model");
            entity.HasIndex(e => e.SourceId, "IX_SourceId");
            entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("models_token");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand")
                .HasDefaultValue(null);

            entity.Property(e => e.CategoriesId)
                .IsRequired()
                .HasColumnName("categories_id");

            entity.Property(e => e.CleanedModel)
                .HasMaxLength(40)
                .HasColumnName("cleaned_model");

            entity.Property(e => e.Confidence)
                .HasMaxLength(1)
                .HasColumnName("confidence")
                .IsRequired();

            entity.Property(e => e.DateModel)
                .HasColumnType("date")
                .HasColumnName("date_model")
                .HasDefaultValue(null);

            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_names")
                .HasDefaultValue(null);

            entity.Property(e => e.FileTitle)
                .HasMaxLength(100)
                .HasColumnName("file_title")
                .HasDefaultValue(null);

            entity.Property(e => e.LocalPath)
                .HasMaxLength(200)
                .HasColumnName("local_path")
                .HasDefaultValue(null);

            entity.Property(e => e.model)
                .HasMaxLength(40)
                .HasColumnName("model")
                .IsRequired();

            entity.Property(e => e.SourceId)
                .IsRequired()
                .HasColumnName("source_id");

            entity.Property(e => e.Token)
                .HasMaxLength(6)
                .HasColumnName("token")
                .HasDefaultValue(null);

            entity.Property(e => e.Version)
                .HasMaxLength(100)
                .HasColumnName("version")
                .HasDefaultValue(null);

            entity.Property(e => e.WebLink)
                .HasMaxLength(200)
                .HasColumnName("web_link")
                .HasDefaultValue(null);
        }
    }
}