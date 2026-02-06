using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace KameraData.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_jobs_id");
            entity.ToTable("jobs", tb => tb.UseSqlOutputClause(false));

            entity.HasIndex(e => new { e.UserId, e.JobNumber, e.JobLink }, "sost_unik").IsUnique();
            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.JobLink)
                .HasMaxLength(57)
                .HasColumnName("job_link");

            entity.Property(e => e.JobNumber)
                .HasMaxLength(555)
                .HasColumnName("job_number");

            entity.Property(e => e.Token)
                .HasMaxLength(555)
                .HasColumnName("token");

            entity.Property(e => e.UpdateRequestStatus)
                .HasMaxLength(555)
                .HasColumnName("update_request_status");

            entity.Property(e => e.UserId)
                .HasColumnType("int")
                .HasColumnName("user_id")
                .IsRequired();

            entity.HasOne(d => d.User)
                .WithMany(p => p.Jobs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jobs_ibfk_1");

        }
    }
}