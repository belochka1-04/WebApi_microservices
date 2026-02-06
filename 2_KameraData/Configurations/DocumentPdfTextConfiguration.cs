using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class DocumentPdfTextConfiguration : IEntityTypeConfiguration<DocumentPdfText>
    {
        public void Configure(EntityTypeBuilder<DocumentPdfText> entity)
        {
            entity.ToTable("DocumentPdfText", tb => tb.UseSqlOutputClause(false));

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.Property(e => e.DocumentId)
                .HasColumnName("DocumentId");

            entity.Property(e => e.PdfText)
                .HasColumnName("PdfText");
        }
    }
}