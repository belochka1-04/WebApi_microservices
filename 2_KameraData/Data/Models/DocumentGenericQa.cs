using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KameraData.Data.Models
{
    [Table("document_generic_qa")]
    [Index(nameof(AnalysisId), Name = "IX_DocumentGenericQA_AnalysisId")]
    [Index(nameof(QuestionId), Name = "IX_DocumentGenericQA_QuestionId")]
    [Index(nameof(Qaid), Name = "IX_document_generic_qa_QAId")]
    public partial class DocumentGenericQa
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Column("QAId")]
        public int Qaid { get; set; }

        [Column("AnalysisId")]
        public int AnalysisId { get; set; }

        [Column("CreatedAt", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(AnalysisId))]
        [InverseProperty(nameof(DocumentAnalysis.DocumentGenericQas))]
        public virtual DocumentAnalysis Analysis { get; set; } = null!;

        [ForeignKey(nameof(Qaid))]
        [InverseProperty(nameof(DocumentQa.DocumentGenericQas))]
        public virtual DocumentQa Qa { get; set; } = null!;

        [ForeignKey(nameof(QuestionId))]
        [InverseProperty(nameof(GenericQuestion.DocumentGenericQas))]
        public virtual GenericQuestion Question { get; set; } = null!;
    }
}
