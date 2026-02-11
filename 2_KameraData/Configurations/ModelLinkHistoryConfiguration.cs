using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ModelLinkHistoryConfiguration : IEntityTypeConfiguration<ModelLinkHistory>
    {
        public void Configure(EntityTypeBuilder<ModelLinkHistory> builder)
        {
            builder.ToTable("model_link_history");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ModelId)
                .HasColumnName("model_id")
                .IsRequired();

            builder.Property(x => x.OldLink)
                .HasColumnName("old_link")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.ChangedAt)
                .HasColumnName("changed_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(x => x.Source)
                .HasColumnName("source")
                .HasMaxLength(100);

            builder.HasOne(x => x.Model)
            .WithMany() // без навигации в ModelTb
            .HasForeignKey(x => x.ModelId)
            .HasConstraintName("FK_model_link_history_model");

        }
    }
}
