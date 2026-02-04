using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KameraData.Data.Models
{
    [Table("document_analysis")]
    [Index(nameof(DocumentId), Name = "IX_document_analysis_DocumentId")]
    public partial class DocumentAnalysis
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DocumentId")]
        public int DocumentId { get; set; }

        [Column("Status")]
        public int Status { get; set; }

        [Column("CreatedAt", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(DocumentId))]
        
        public virtual Document Document { get; set; } = null!;

        [InverseProperty(nameof(DocumentGenericQa.Analysis))]
        public virtual ICollection<DocumentGenericQa> DocumentGenericQas { get; set; } = [];

        [InverseProperty(nameof(DocumentQa.Analysis))]
        public virtual ICollection<DocumentQa> DocumentQas { get; set; } = [];
    }
}
