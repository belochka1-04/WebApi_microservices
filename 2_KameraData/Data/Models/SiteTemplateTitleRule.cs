using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models
{
    [Table("site_template_title_rules")]
    public class SiteTemplateTitleRule
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("site_template_id")]
        public int SiteTemplateId { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

        [Column("pattern")]
        public string Pattern { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // navigation (в БД колонки нет — Column не нужен)
        public SiteTemplate? SiteTemplate { get; set; }
    }
}
