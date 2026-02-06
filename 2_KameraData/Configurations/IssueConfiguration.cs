using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class IssueConfiguration : IEntityTypeConfiguration<Issue>
    {
        public void Configure(EntityTypeBuilder<Issue> entity)
        {
            entity.ToTable("issues");
            entity.HasKey(e => e.IssueId);

            entity.Property(e => e.IssueId)
                .HasColumnName("issue_id")
                .UseIdentityColumn(1, 1);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.Timestamp)
                .HasColumnName("timestamp")
                .HasColumnType("datetime");

            entity.Property(e => e.JobId)
                .HasColumnName("job_id");

            entity.Property(e => e.ScreenName)
                .HasColumnName("screen_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.UserInput)
                .HasColumnName("user_input")
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.BotResponse)
                .HasColumnName("bot_response")
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Buttons)
                .HasColumnName("buttons")
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.FilePath)
                .HasColumnName("file_path")
                .HasMaxLength(255);

            entity.Property(e => e.FileType)
                .HasColumnName("file_type")
                .HasMaxLength(50);

            entity.Property(e => e.RelatedDocsCount)
                .HasColumnName("related_docs_count");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(e => e.Mode)
                .HasColumnName("Mode")
                .HasMaxLength(50);

            entity.Property(e => e.IssueType)
                .HasColumnName("issue_type")
                .HasMaxLength(100);

            entity.Property(e => e.IssueDetail)
                .HasColumnName("issue_detail")
                .HasMaxLength(1000);

            entity.Property(e => e.PartsAndReplacesId)
                .HasColumnName("parts_and_replaces_id");
        }
    }
}
