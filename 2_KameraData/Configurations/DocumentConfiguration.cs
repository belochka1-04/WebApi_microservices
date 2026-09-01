using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_Document");
            entity.ToTable("Document", tb => tb.UseSqlOutputClause(false));

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("Title")
                .HasDefaultValue(null);

            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .HasColumnName("Link")
                .HasDefaultValue(null);

            //entity.Property(e => e.ModelId)
            //    .HasColumnName("ModelId")
            //    .HasDefaultValue(null);

            entity.Property(e => e.DocumentTypeId)
                .HasColumnName("DocumentTypeId")
                .HasDefaultValue(null);

            entity.Property(e => e.SiteId)
                .HasColumnName("SiteId")
                .HasDefaultValue(null);

            entity.Property(e => e.DateModel)
                .HasColumnType("datetime")
                .HasColumnName("date_model")
                .HasDefaultValue(null);

            entity.Property(e => e.Version)
                .HasMaxLength(100)
                .HasColumnName("version")
                .HasDefaultValue(null);

            entity.Property(e => e.LocalPath)
                .HasColumnName("LocalPath");

            entity.Property(e => e.ModifiedAt)
                .HasColumnName("ModifiedAt");

            entity.Property(e => e.Words)
                .HasColumnName("Words");

            entity.Property(e => e.token)
                .HasColumnName("token");

            entity.Property(e => e.Link2)
                .HasMaxLength(255)
                .HasColumnName("Link2");

            entity.Property(e => e.DocStatus)
                .HasColumnName("DocStatus");

            entity.Property(e => e.OldId)
                .HasColumnName("old_id");

            entity.Property(e => e.MId)
                .HasColumnName("m_id");

            entity.HasOne(d => d.DocumentType)
                .WithMany()
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
