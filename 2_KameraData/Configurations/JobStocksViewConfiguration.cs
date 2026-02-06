using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobStocksViewConfiguration : IEntityTypeConfiguration<JobStocksView>
    {
        public void Configure(EntityTypeBuilder<JobStocksView> entity)
        {
            entity.HasNoKey();
            entity.ToView("job_stocks_view");

            entity.Property(e => e.Id).HasColumnType("int").HasColumnName("id");
            entity.Property(e => e.JobDescription).HasColumnType("nvarchar(max)").HasColumnName("job_description");
            entity.Property(e => e.JobId).HasColumnType("int").HasColumnName("job_id");
            entity.Property(e => e.JobNotes).HasColumnType("nvarchar(max)").HasColumnName("job_notes");
            entity.Property(e => e.MainPartNumber).HasMaxLength(20).HasColumnName("main_part_number");
            entity.Property(e => e.MatchPartNumber).HasMaxLength(30).HasColumnName("match_part_number");
            entity.Property(e => e.MnJobId).HasColumnType("int").HasColumnName("mn_job_id");
            entity.Property(e => e.ModelNumber).HasMaxLength(100).HasColumnName("model_number");
            entity.Property(e => e.NotFountModel).HasMaxLength(100).HasColumnName("not_fount_model");
            entity.Property(e => e.PartId).HasColumnType("int").HasColumnName("part_id");
            entity.Property(e => e.PartName).HasMaxLength(70).HasColumnName("part_name");
            entity.Property(e => e.PartNumber).HasMaxLength(50).HasColumnName("part_number");
            entity.Property(e => e.PartsAndReplacesId).HasColumnType("int").HasColumnName("parts_and_replaces_id");
            entity.Property(e => e.Replaces).HasColumnName("replaces");
            entity.Property(e => e.ResponseText).HasColumnType("nvarchar(max)").HasColumnName("response_text");
            entity.Property(e => e.StockName).HasMaxLength(30).HasColumnName("stock_name");
            entity.Property(e => e.Token).HasColumnType("nvarchar(max)").HasColumnName("token");
            entity.Property(e => e.UserStockId).HasColumnType("int").HasColumnName("user_stock_id");
        }
    }
}
