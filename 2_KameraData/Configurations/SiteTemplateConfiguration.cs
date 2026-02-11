using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Configurations
{
    public class SiteTemplateConfiguration
    {
        public void Configure(EntityTypeBuilder<SiteTemplate> builder)
        {
            builder.ToTable("site_templates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.SiteId)
                .HasColumnName("site_id")
                .IsRequired();

            builder.Property(x => x.SiteName)
                .HasColumnName("site_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.UrlPattern)
                .HasColumnName("url_pattern")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.TitlePattern)
                .HasColumnName("title_pattern")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.NormalizationRules)
                .HasColumnName("normalization_rules");

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime");

            builder.Property(x => x.TitleStripSuffix)
                .HasColumnName("title_strip_suffix")
                .HasMaxLength(100);

            builder.Property(x => x.TitleRequiredSpaceCount)
                .HasColumnName("title_required_space_count");

            builder.Property(x => x.TitleMode)
                .HasColumnName("title_mode")
                .HasMaxLength(50);

            builder.Property(x => x.CompositeBrands)
                .HasColumnName("composite_brands")
                .HasMaxLength(500);
        }
    }
}
