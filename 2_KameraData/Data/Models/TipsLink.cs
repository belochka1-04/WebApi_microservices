using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    [Table("tips_links")]
    public partial class TipsLink
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("prefix_text")]
        [StringLength(100)]
        public string PrefixText { get; set; } = null!;

        [Column("main_text")]
        [StringLength(200)]
        public string MainText { get; set; } = null!;

        [Column("url")]
        [StringLength(500)]
        public string? Url { get; set; }

        [Column("remaining_shows")]
        public int RemainingShows { get; set; }
    }
}
