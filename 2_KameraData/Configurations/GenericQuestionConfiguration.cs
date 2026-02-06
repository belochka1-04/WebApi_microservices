using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class GenericQuestionConfiguration : IEntityTypeConfiguration<GenericQuestion>
    {
        public void Configure(EntityTypeBuilder<GenericQuestion> entity)
        {
            entity.ToTable("generic_questions");

            entity.Property(e => e.Id)
                .HasColumnName("Id");

            entity.Property(e => e.Question)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Question");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime");

            entity.HasMany(d => d.DocumentGenericQas)
                .WithOne(p => p.Question)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_DocumentGenericQa_GenericQuestion");
        }
    }
}