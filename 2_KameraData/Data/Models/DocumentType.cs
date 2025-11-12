using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;

[Table("DocumentType")]
public partial class DocumentType
{
    public int Id { get; set; }

    [Column("Title")]
    public string? Title { get; set; } // Строка, может быть null
}

