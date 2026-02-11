using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class SiteTemplate
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; } = null!;

        public string UrlPattern { get; set; } = null!;

        public string TitlePattern { get; set; } = null!;

        public string? NormalizationRules { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? TitleStripSuffix { get; set; }

        public int? TitleRequiredSpaceCount { get; set; }

        public string? TitleMode { get; set; }

        public string? CompositeBrands { get; set; }
    }
}
