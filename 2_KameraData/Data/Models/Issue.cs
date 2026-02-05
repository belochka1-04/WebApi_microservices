using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    [Table("issues")]
    public partial class Issue
    {
        [Key]
        [Column("issue_id")]
        public int IssueId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }

        [Column("job_id")]
        public int? JobId { get; set; }

        [Column("screen_name")]
        [StringLength(255)]
        public string ScreenName { get; set; } = null!;

        [Column("user_input")]
        public string? UserInput { get; set; }

        [Column("bot_response")]
        public string? BotResponse { get; set; }

        [Column("buttons")]
        public string? Buttons { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("file_path")]
        [StringLength(255)]
        public string? FilePath { get; set; }

        [Column("file_type")]
        [StringLength(50)]
        public string? FileType { get; set; }

        [Column("related_docs_count")]
        public int? RelatedDocsCount { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(50)]
        public string? Mode { get; set; }

        [Column("issue_type")]
        [StringLength(100)]
        public string? IssueType { get; set; }

        [Column("issue_detail")]
        [StringLength(1000)]
        public string? IssueDetail { get; set; }

        [Column("parts_and_replaces_id")]
        public int? PartsAndReplacesId { get; set; }
    }
}
