using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{

    [Table("onboarding_videos")]
    public partial class OnboardingVideo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("branch")]
        [StringLength(50)]
        public string Branch { get; set; } = null!;

        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        [Column("video_url")]
        [StringLength(1000)]
        public string Url { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
