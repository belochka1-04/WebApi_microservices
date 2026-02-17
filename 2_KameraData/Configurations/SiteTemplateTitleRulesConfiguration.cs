using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi_GoogleSearchTemplatesService.Data.Configurations
{
    public class SiteTemplateTitleRulesConfiguration : IEntityTypeConfiguration<SiteTemplateTitleRule>
    {
        public void Configure(EntityTypeBuilder<SiteTemplateTitleRule> builder)
        {
            builder.ToTable("site_template_title_rules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SiteTemplateId).HasColumnName("site_template_id").IsRequired();
            builder.Property(x => x.Priority).HasColumnName("priority").IsRequired();

            builder.Property(x => x.Pattern)
                .HasColumnName("pattern")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(x => x.SiteTemplate)
                .WithMany(t => t.TitleRules)
                .HasForeignKey(x => x.SiteTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.SiteTemplateId, x.IsActive, x.Priority })
                .HasDatabaseName("IX_site_template_title_rules_template_active_priority");
        }
    }
}
