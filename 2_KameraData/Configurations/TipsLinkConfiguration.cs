using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class TipsLinkConfiguration : IEntityTypeConfiguration<TipsLink>
    {
        public void Configure(EntityTypeBuilder<TipsLink> entity)
        {
            entity.ToTable("tips_links");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn(1, 1);

            entity.Property(e => e.PrefixText)
                .HasColumnName("prefix_text")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.MainText)
                .HasColumnName("main_text")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Url)
                .HasColumnName("url")
                .HasMaxLength(500);

            entity.Property(e => e.RemainingShows)
                .HasColumnName("remaining_shows")
                .IsRequired();

            entity.HasIndex(e => e.RemainingShows)
                .HasDatabaseName("IX_tips_links_remaining_shows");
        }
    }
}