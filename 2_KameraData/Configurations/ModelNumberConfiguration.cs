using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelNumberConfiguration : IEntityTypeConfiguration<ModelNumber>
    {
        public void Configure(EntityTypeBuilder<ModelNumber> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_model_numbers_id");
            entity.ToTable("model_numbers", tb => tb.UseSqlOutputClause(false));

            entity.HasIndex(e => e.JobId, "IX_JobId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Confirmed)
                .HasMaxLength(2)
                .HasColumnName("confirmed")
                .IsRequired();

            entity.Property(e => e.GotForWorkAt)
                .HasColumnType("datetime2")
                .HasColumnName("got_for_work_at");

            entity.Property(e => e.SerialNumber)
                .HasColumnName("serial_number");

            entity.Property(e => e.BrandId)
                .HasColumnName("brand_id");

            entity.Property(e => e.Brand)
                .HasColumnName("brand");

            entity.Property(e => e.JobId)
                .IsRequired()
                .HasColumnName("job_id");

            entity.Property(e => e.ModelNumber1)
                .HasMaxLength(100)
                .HasColumnName("model_number")
                .IsRequired();

            entity.Property(e => e.DocCounter)
                .HasColumnName("doc_counter");

            entity.Property(e => e.jdConfirmed)
                .HasColumnName("jd_confirmed");

            entity.Property(e => e.CleanedModel)
                .HasColumnName("cleaned_model");

            entity.Property(e => e.WorkerId)
                .HasColumnName("worker_id")
                .HasMaxLength(128);

            entity.Property(e => e.LeaseUntil)
                .HasColumnName("lease_until")
                .HasColumnType("datetime2(3)");

            entity.Property(e => e.AttemptCount)
                .HasColumnName("attempt_count")
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(e => e.LastError)
                .HasColumnName("last_error")
                .HasMaxLength(2000);

            entity.HasOne(d => d.Job)
                .WithMany(p => p.ModelNumbers)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_model_numbers_Job");
        }
    }
}
