using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace KameraData.Data.Models;

[Table("DocumentPdfText")]
public partial class DocumentPdfText
{
    public int Id { get; set; }

    [Column("DocumentId")]
    public int? DocumentId { get; set; } // Строка, может быть null

    [Column("PdfText")]
    public string? PdfText { get; set; } // Строка, может быть null

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; } // Дата, может быть null

}