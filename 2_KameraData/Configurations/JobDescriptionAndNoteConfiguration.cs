using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobDescriptionAndNoteConfiguration : IEntityTypeConfiguration<JobDescriptionAndNote>
    {
        public void Configure(EntityTypeBuilder<JobDescriptionAndNote> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_job_description_and_notes_id");
            entity.ToTable("job_description_and_notes");

            entity.HasIndex(e => e.JobId, "job_id").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.JobDescription)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("job_description")
                .IsRequired(false);

            entity.Property(e => e.JobId)
                .HasColumnType("int")
                .HasColumnName("job_id");

            entity.Property(e => e.JobNotes)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("job_notes")
                .IsRequired(false);

            entity.Property(e => e.PicCounter)
                .HasColumnType("int")
                .HasColumnName("pic_counter")
                .IsRequired(false);

            entity.Property(e => e.Status)
                .HasColumnType("nvarchar(1)")
                .HasColumnName("status")
                .IsRequired(false);

            entity.Property(e => e.Brand)
                .HasColumnName("brand");

            entity.Property(e => e.Model)
                .HasColumnName("model");

            entity.Property(e => e.SerialNumber)
                .HasColumnName("serial_number");

            entity.Property(e => e.OcrText)
                .HasColumnName("ocr_text");

            entity.Property(e => e.IsRatingPlatePercentage)
                .HasColumnName("is_rating_plate_percentage");

            entity.Property(e => e.CompletionCost)
                .HasColumnName("completion_cost");

            entity.Property(e => e.ImageTokens)
                .HasColumnName("image_tokens");

            entity.Property(e => e.StickerLink)
                .HasColumnName("sticker_link");

            entity.Property(e => e.OriginalBrand)
                .HasColumnName("original_brand");

            entity.Property(e => e.GoogleConfirmed)
              .HasColumnName("google_confirmed");

            entity.HasOne(d => d.Job)
                .WithOne(p => p.JobDescriptionAndNote)
                .HasForeignKey<JobDescriptionAndNote>(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("job_desc");
        }
    }
}
