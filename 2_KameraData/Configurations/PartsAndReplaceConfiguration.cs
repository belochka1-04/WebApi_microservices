using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PartsAndReplaceConfiguration : IEntityTypeConfiguration<PartsAndReplace>
    {
        public void Configure(EntityTypeBuilder<PartsAndReplace> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_parts_and_replaces");
            entity.ToTable("parts_and_replaces");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.GotForWorkAt)
                .HasColumnType("datetime")
                .HasColumnName("got_for_work_at");

            entity.Property(e => e.MainPartNumber)
                .HasMaxLength(40)
                .HasColumnName("main_part_number");

            entity.Property(e => e.PartName)
                .HasMaxLength(140)
                .HasColumnName("part_name");

            entity.Property(e => e.PicBase64)
                .HasColumnName("pic_base64");

            entity.Property(e => e.Replaces)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("replaces");

            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");

            entity.Property(e => e.Brand)
                .HasMaxLength(510)
                .HasColumnName("brand");

            entity.Property(e => e.Pic)
                .HasColumnName("pic");

            entity.Property(e => e.PicLink)
                .HasColumnName("pic_link");

            entity.Property(e => e.PhotoStatus)
                .HasColumnName("Photo_status");

            entity.Property(e => e.DateUpdate)
                .HasColumnType("datetime")
                .HasColumnName("date_update");

            entity.Property(e => e.UserCreated)
                .HasMaxLength(510)
                .HasColumnName("user_created");

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("date_created");

            entity.Property(e => e.RecordId)
                .HasColumnName("record_id");

            entity.Property(e => e.MainPartNumberNorm)
                .HasMaxLength(40)
                .HasColumnName("main_part_number_norm");

            entity.Property(e => e.PicUrl)
                .HasColumnName("pic_url");

            entity.Property(e => e.OrderFind)
                .HasColumnName("order_find");

            entity.Property(e => e.PartsWorkerId)
                .HasMaxLength(256)
                .HasColumnName("parts_worker_id");

            entity.Property(e => e.PartsLeaseUntilUtc)
                .HasColumnType("datetime2(3)")
                .HasColumnName("parts_lease_until_utc");

            entity.Property(e => e.PartsAttemptCount)
                .HasColumnName("parts_attempt_count");

            entity.Property(e => e.PartsProcessingStartedAtUtc)
                .HasColumnType("datetime2(3)")
                .HasColumnName("parts_processing_started_at_utc");

            entity.Property(e => e.PartsProcessingCompletedAtUtc)
                .HasColumnType("datetime2(3)")
                .HasColumnName("parts_processing_completed_at_utc");

            entity.Property(e => e.PartsProcessingLastError)
                .HasMaxLength(4000)
                .HasColumnName("parts_processing_last_error");
        }
    }
}
