using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi_JobService.Infrastructure.Persistence.Entities
{
    [Table("jobs")]
    public partial class Job
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // [user_id] [int] NOT NULL
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("job_number")]
        [StringLength(555)]
        public string? JobNumber { get; set; }

        [Column("job_link")]
        [StringLength(57)]
        public string? JobLink { get; set; }

        [Column("token")]
        [StringLength(555)]
        public string? Token { get; set; }

        [Column("update_request_status")]
        [StringLength(555)]
        public string? UpdateRequestStatus { get; set; }

        // [created_at] [datetime] NULL
        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        // Навигационные свойства

        public virtual JobDescriptionAndNote? JobDescriptionAndNote { get; set; }

        public virtual ICollection<JobDoc> JobDocs { get; set; } = new List<JobDoc>();

        public virtual ICollection<JobStock> JobStocks { get; set; } = new List<JobStock>();


        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
