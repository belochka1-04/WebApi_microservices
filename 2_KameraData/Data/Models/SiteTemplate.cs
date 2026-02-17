using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models
{
    [Table("site_templates")] // точное имя таблицы в БД
    public class SiteTemplate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("site_id")]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("site_name")]
        public string SiteName { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        [Column("url_pattern")]
        public string UrlPattern { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        [Column("title_pattern")]
        public string TitlePattern { get; set; } = null!;

        [Column("normalization_rules")]
        public string? NormalizationRules { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        [Column("title_strip_suffix")]
        public string? TitleStripSuffix { get; set; }

        [Column("title_required_space_count")]
        public int? TitleRequiredSpaceCount { get; set; }

        [MaxLength(50)]
        [Column("title_mode")]
        public string? TitleMode { get; set; }

        [MaxLength(500)]
        [Column("composite_brands")]
        public string? CompositeBrands { get; set; }

        public ICollection<SiteTemplateTitleRule> TitleRules { get; set; } = new List<SiteTemplateTitleRule>();
    }
}
