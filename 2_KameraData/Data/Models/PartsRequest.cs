using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models
{
    [Table("Parts_request")]
    public class PartsRequest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("stock_id")]
        public int? StockId { get; set; }

        [Column("photo_path")]
        [MaxLength(255)]
        public string? PhotoPath { get; set; }

        [Column("recognized_part_number")]
        [MaxLength(50)]
        public string? RecognizedPartNumber { get; set; }

        [Column("parts_and_replaces_id")]
        public int? PartsAndReplacesId { get; set; }

        [Column("status")]
        public byte? Status { get; set; }  // SQL TINYINT

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("job_id")]
        public int? JobId { get; set; }
    }
}
