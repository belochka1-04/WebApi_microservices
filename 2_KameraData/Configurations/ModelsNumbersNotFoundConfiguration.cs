using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelsNumbersNotFoundConfiguration : IEntityTypeConfiguration<ModelsNumbersNotFound>
    {
        public void Configure(EntityTypeBuilder<ModelsNumbersNotFound> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_models_numbers_not_found_id");
            entity.ToTable("models_numbers_not_found");

            entity.HasIndex(e => e.JobId, "IX_JobId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.JobId)
                .IsRequired()
                .HasColumnName("job_id");

            entity.Property(e => e.ModelNumber)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("model_number");

            entity.HasOne(d => d.Job)
                .WithMany(p => p.ModelsNumbersNotFounds)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_models_numbers_not_found_Job");
        }
    }
}