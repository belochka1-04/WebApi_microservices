using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PartsRequestConfiguration : IEntityTypeConfiguration<PartsRequest>
    {
        public void Configure(EntityTypeBuilder<PartsRequest> entity)
        {
            entity.ToTable("Parts_request", tb => tb.UseSqlOutputClause(false));
            entity.HasKey(e => e.Id).HasName("PK_Parts_request_id");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.StockId)
                .HasColumnName("stock_id");

            entity.Property(e => e.PhotoPath)
                .HasMaxLength(255)
                .HasColumnName("photo_path");

            entity.Property(e => e.RecognizedPartNumber)
                .HasMaxLength(50)
                .HasColumnName("recognized_part_number");

            entity.Property(e => e.PartsAndReplacesId)
                .HasColumnName("parts_and_replaces_id");

            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            entity.Property(e => e.JobId)
                .HasColumnName("job_id");
        }
    }
}