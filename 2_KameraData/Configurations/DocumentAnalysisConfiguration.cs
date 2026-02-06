using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class DocumentAnalysisConfiguration : IEntityTypeConfiguration<DocumentAnalysis>
    {
        public void Configure(EntityTypeBuilder<DocumentAnalysis> entity)
        {
            entity.ToTable("document_analysis");

            entity.HasIndex(e => e.DocumentId, "IX_document_analysis_DocumentId");

            entity.Property(e => e.Id)
                .HasColumnName("Id");

            entity.Property(e => e.DocumentId)
                .HasColumnName("DocumentId");

            entity.Property(e => e.Status)
                .HasColumnName("Status");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Document);

            entity.HasMany(d => d.DocumentGenericQas)
                .WithOne(p => p.Analysis)
                .HasForeignKey(d => d.AnalysisId)
                .HasConstraintName("FK_DocumentGenericQa_Analysis");

            entity.HasMany(d => d.DocumentQas)
                .WithOne(p => p.Analysis)
                .HasForeignKey(d => d.AnalysisId)
                .HasConstraintName("FK_DocumentQa_Analysis");
        }
    }
}