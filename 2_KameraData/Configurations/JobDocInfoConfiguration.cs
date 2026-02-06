using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobDocInfoConfiguration : IEntityTypeConfiguration<JobDocInfo>
    {
        public void Configure(EntityTypeBuilder<JobDocInfo> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_job_docs_info_Id");
            entity.ToTable("job_docs_info");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.SourceId)
                .IsRequired()
                .HasColumnName("source_id");

            entity.Property(e => e.JobId)
                .IsRequired()
                .HasColumnName("job_id");

            entity.Property(e => e.ModelsNumberId)
                .IsRequired()
                .HasColumnName("models_number_id");

            entity.Property(e => e.ModelId)
                .HasColumnName("model_id");

            entity.Property(e => e.LocalPath)
                .HasMaxLength(200)
                .HasColumnName("local_path")
                .HasDefaultValue(null);
        }
    }
}