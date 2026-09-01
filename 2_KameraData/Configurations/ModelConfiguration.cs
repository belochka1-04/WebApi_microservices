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
                .HasMaxLength(150)
                .HasColumnName("brand")
                .HasDefaultValue(null);

            entity.Property(e => e.CategoriesId)
                .HasColumnName("categories_id");

            entity.Property(e => e.CleanedModel)
                .HasMaxLength(255)
                .HasColumnName("cleaned_model");

            entity.Property(e => e.Confidence)
                .HasMaxLength(2)
                .HasColumnName("confidence");

            entity.Property(e => e.DateModel)
                .HasColumnType("datetime")
                .HasColumnName("date_model")
                .HasDefaultValue(null);

            entity.Property(e => e.FileName)
                .HasColumnName("file_names")
                .HasDefaultValue(null);

            entity.Property(e => e.FileTitle)
                .HasMaxLength(100)
                .HasColumnName("file_title")
                .HasDefaultValue(null);

            entity.Property(e => e.LocalPath)
                .HasColumnName("local_path")
                .HasDefaultValue(null);

            entity.Property(e => e.model)
                .HasMaxLength(150)
                .HasColumnName("model");

            entity.Property(e => e.SourceId)
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
                .HasMaxLength(255)
                .HasColumnName("web_link")
                .HasDefaultValue(null);
        }
    }
}
