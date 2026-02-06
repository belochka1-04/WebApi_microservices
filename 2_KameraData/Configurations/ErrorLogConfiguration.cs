using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> entity)
        {
            entity.ToTable("Error_log");
            entity.HasKey(e => e.id);

            entity.Property(e => e.id)
                .HasColumnName("id")
                .UseIdentityColumn();

            entity.Property(e => e.user_id)
                .HasColumnName("user_id")
                .IsRequired(false);

            entity.Property(e => e.timestamp)
                .HasColumnName("timestamp")
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.error_type)
                .HasColumnName("error_type")
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired(false);

            entity.Property(e => e.details)
                .HasColumnName("details")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.action_taken)
                .HasColumnName("action_taken")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.screen_name)
                .HasColumnName("screen_name")
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.request_data)
                .HasColumnName("request_data")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.job_id)
                .HasColumnName("job_id")
                .HasColumnType("int")
                .IsRequired(false);

            entity.Property(e => e.status)
                .HasColumnName("status")
                .HasColumnType("nvarchar")
                .HasMaxLength(10)
                .IsRequired(false);

            entity.Property(e => e.script_name)
                .HasColumnName("script_name")
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired(true);

            entity.Property(e => e.error_message)
                .HasColumnName("error_message")
                .HasColumnType("nvarchar(max)")
                .IsRequired(true);

            entity.Property(e => e.error_time)
                .HasColumnName("error_time")
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.stock_id)
                .HasColumnName("stock_id")
                .HasColumnType("int")
                .IsRequired(false);

            entity.Property(e => e.additional_data1)
                .HasColumnName("additional_data1")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.additional_data2)
                .HasColumnName("additional_data2")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.additional_data3)
                .HasColumnName("additional_data3")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.additional_data4)
                .HasColumnName("additional_data4")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(e => e.additional_data5)
                .HasColumnName("additional_data5")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);
        }
    }
}