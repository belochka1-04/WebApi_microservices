using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class DocumentGenericQaConfiguration : IEntityTypeConfiguration<DocumentGenericQa>
    {
        public void Configure(EntityTypeBuilder<DocumentGenericQa> entity)
        {
            entity.ToTable("document_generic_qa");

            entity.HasIndex(e => e.AnalysisId, "IX_DocumentGenericQA_AnalysisId");
            entity.HasIndex(e => e.QuestionId, "IX_DocumentGenericQA_QuestionId");
            entity.HasIndex(e => e.Qaid, "IX_document_generic_qa_QAId");

            entity.Property(e => e.Id)
                .HasColumnName("Id");

            entity.Property(e => e.QuestionId)
                .HasColumnName("QuestionId");

            entity.Property(e => e.Qaid)
                .HasColumnName("QAId");

            entity.Property(e => e.AnalysisId)
                .HasColumnName("AnalysisId");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Analysis)
                .WithMany(p => p.DocumentGenericQas)
                .HasForeignKey(d => d.AnalysisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentGenericQa_Analysis");

            entity.HasOne(d => d.Qa)
                .WithMany(p => p.DocumentGenericQas)
                .HasForeignKey(d => d.Qaid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentGenericQa_Qa");

            entity.HasOne(d => d.Question)
                .WithMany(p => p.DocumentGenericQas)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DocumentGenericQa_GenericQuestion");
        }
    }
}