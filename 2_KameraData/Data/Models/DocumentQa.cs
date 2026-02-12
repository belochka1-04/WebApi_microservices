using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models
{
    [Table("document_qa")]
    [Index(nameof(AnalysisId), Name = "IX_DocumentQA_AnalysisId")]
    public partial class DocumentQa
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        [Column("Question")]
        public string Question { get; set; } = null!;

        [Column("Answer")]
        public string? Answer { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Column("AnalysisId")]
        public int AnalysisId { get; set; }

        [Column("CreatedAt", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<DocumentGenericQa> DocumentGenericQas { get; set; } = [];

        [ForeignKey(nameof(AnalysisId))]
        public virtual DocumentAnalysis Analysis { get; set; } = null!;
    }
}
