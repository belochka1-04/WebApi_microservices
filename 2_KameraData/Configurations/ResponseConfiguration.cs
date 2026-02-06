using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_Responses_id");
            entity.ToTable("Responses");

            entity.HasIndex(e => e.CrmId, "fk_responses_crm");
            entity.HasIndex(e => e.JobId, "job_id");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.AttemptCount)
                .HasDefaultValue(0)
                .HasColumnType("int")
                .HasColumnName("attempt_count");

            entity.Property(e => e.CrmId)
                .HasColumnType("int")
                .HasColumnName("CRM_id");

            entity.Property(e => e.JobId)
                .IsRequired()
                .HasColumnType("int")
                .HasColumnName("job_id");

            entity.Property(e => e.JobLink)
                .HasMaxLength(255)
                .HasColumnName("job_link");

            entity.Property(e => e.ResponseText)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasColumnName("response_text");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnName("status");

            entity.Property(e => e.TgStatus)
                .IsRequired()
                .HasColumnName("TG_status");

            entity.Property(e => e.UserId)
                .HasColumnType("int")
                .HasColumnName("user_id");

            entity.Property(e => e.WhatsappStatus)
                .IsRequired()
                .HasColumnName("Whatsapp_status");

            entity.HasOne(d => d.Job)
                .WithMany(p => p.Responses)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Responses_ibfk_1");
        }
    }
}