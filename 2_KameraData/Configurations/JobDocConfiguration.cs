using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobDocConfiguration : IEntityTypeConfiguration<JobDoc>
    {
        public void Configure(EntityTypeBuilder<JobDoc> entity)
        {
            entity.ToTable("job_docs");
            entity.HasKey(e => e.Id).HasName("PK_job_docs");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.JobId)
                .HasColumnName("job_id")
                .IsRequired();

            entity.Property(e => e.ModelsId)
                .HasColumnName("models_id")
                .IsRequired();

            entity.Property(e => e.CleanedModel)
                .HasColumnName("cleaned_model");

            entity.Property(e => e.DocumentType)
                .HasColumnName("DocumentType")
                .HasMaxLength(255);

            entity.Property(e => e.Site)
                .HasColumnName("Site");

            entity.Property(e => e.Part_counter)
                .HasColumnName("Part_counter");

            entity.Property(e => e.Status)
                .HasColumnName("Status");

            entity.Property(e => e.siteState)
                .HasColumnName("siteState");

            entity.Property(e => e.docState)
                .HasColumnName("docState");

            entity.Property(e => e.partCountState)
                .HasColumnName("partCountState");

            entity.Property(e => e.Document_Id)
                .HasColumnName("Document_Id");

            entity.Property(e => e.CPСountState)
                .HasColumnName("CPСountState");

            entity.Property(e => e.clicked)
                .HasColumnName("clicked");

            entity.HasOne(d => d.Job)
                .WithMany(p => p.JobDocs)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_job_docs_jobs");
        }
    }
}