using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobStockConfiguration : IEntityTypeConfiguration<JobStock>
    {
        public void Configure(EntityTypeBuilder<JobStock> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_job_stocks_id");
            entity.ToTable("job_stocks");

            entity.HasIndex(e => e.JobId, "IX_JobId");
            entity.HasIndex(e => e.UserStockId, "IX_UserStockId");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.JobId)
                .IsRequired()
                .HasColumnName("job_id");

            entity.Property(e => e.MatchPartNumber)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("match_part_number");

            entity.Property(e => e.UserStockId)
                .IsRequired()
                .HasColumnName("user_stock_id");

            entity.HasOne(d => d.Job)
                .WithMany(p => p.JobStocks)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_JobStocks_Job");

            entity.HasOne(d => d.UserStock)
                .WithMany(p => p.JobStocks)
                .HasForeignKey(d => d.UserStockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_JobStocks_UserStock");
        }
    }
}