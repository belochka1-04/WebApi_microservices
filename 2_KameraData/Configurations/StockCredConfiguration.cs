using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class StockCredConfiguration : IEntityTypeConfiguration<StockCred>
    {
        public void Configure(EntityTypeBuilder<StockCred> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_stock_creds_id");
            entity.ToTable("stock_creds");

            entity.HasIndex(e => e.ContactTypesId, "contact_types_id");
            entity.HasIndex(e => e.DocTypesId, "doc_types_id");
            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.ContactAddress)
                .HasMaxLength(30)
                .HasColumnName("contact_address");

            entity.Property(e => e.ContactTypesId)
                .IsRequired()
                .HasColumnName("contact_types_id");

            entity.Property(e => e.DocTypesId)
                .HasColumnName("doc_types_id");

            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasDefaultValue("partsbot@gmail.com")
                .HasColumnName("email");

            entity.Property(e => e.ExcelColumn)
                .HasMaxLength(2)
                .HasColumnName("excel_column");

            entity.Property(e => e.FileName)
                .HasMaxLength(300)
                .HasColumnName("file_name");

            entity.Property(e => e.Partmanager)
                .HasMaxLength(40)
                .HasColumnName("part_manager");

            entity.Property(e => e.RecordsCount)
                .HasDefaultValue(0)
                .HasColumnName("records_count");

            entity.Property(e => e.ReplaceRecords)
                .HasDefaultValue(0)
                .HasColumnName("replace_records");

            entity.Property(e => e.StockLink)
                .HasMaxLength(300)
                .HasColumnName("stock_link");

            entity.Property(e => e.StockName)
                .HasMaxLength(30)
                .HasColumnName("stock_name");

            entity.Property(e => e.SyncFreq)
                .HasMaxLength(5)
                .HasDefaultValue("60")
                .HasColumnName("sync_freq");

            entity.Property(e => e.SyncSwitch)
                .HasDefaultValue("ON")
                .HasColumnName("sync_switch");

            entity.Property(e => e.UpdateStatus)
                .HasColumnName("update_status");

            entity.Property(e => e.UpdateTime)
                .HasColumnType("datetime")
                .HasColumnName("update_time");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            entity.Property(e => e.MaxRowsPerUpload)
                .HasColumnName("max_rows_per_upload");

            entity.HasOne(d => d.User)
                .WithMany(p => p.StockCreds)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("stock_creds_ibfk_1");
        }
    }
}
