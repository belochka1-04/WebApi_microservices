using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class DocumentQaConfiguration : IEntityTypeConfiguration<DocumentQa>
    {
        public void Configure(EntityTypeBuilder<DocumentQa> entity)
        {
            entity.ToTable("document_qa");

            entity.HasIndex(e => e.AnalysisId, "IX_DocumentQA_AnalysisId");

            entity.Property(e => e.Id)
                .HasColumnName("Id");

            entity.Property(e => e.Question)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("Question");

            entity.Property(e => e.Answer)
                .HasColumnName("Answer");

            entity.Property(e => e.Status)
                .HasColumnName("Status");

            entity.Property(e => e.AnalysisId)
                .HasColumnName("AnalysisId");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Analysis)
                .WithMany(p => p.DocumentQas)
                .HasForeignKey(d => d.AnalysisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentQa_Analysis");

            entity.HasMany(d => d.DocumentGenericQas)
                .WithOne(p => p.Qa)
                .HasForeignKey(d => d.Qaid)
                .HasConstraintName("FK_DocumentGenericQa_Qa");
        }
    }
}