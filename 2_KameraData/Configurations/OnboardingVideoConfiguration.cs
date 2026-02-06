using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class OnboardingVideoConfiguration : IEntityTypeConfiguration<OnboardingVideo>
    {
        public void Configure(EntityTypeBuilder<OnboardingVideo> entity)
        {
            entity.ToTable("onboarding_videos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn(1, 1);

            entity.Property(e => e.Branch)
                .HasColumnName("branch")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Url)
                .HasColumnName("video_url")
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}