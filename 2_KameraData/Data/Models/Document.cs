using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace KameraData.Data.Models;

[Table("Document")]
public partial class Document
{
    public int Id { get; set; }

    [Column("Title")]
    public string? Title { get; set; } // Строка, может быть null

    [Column("Link")]
    public string? Link { get; set; } // Строка, может быть null

    [Column("DocumentTypeId")]
    public int? DocumentTypeId { get; set; } // Идентификатор типа документа, может быть null

    [Column("SiteId")]
    public int? SiteId { get; set; } // Идентификатор сайта, может быть null

    [Column("date_model")]
    public DateTime? DateModel { get; set; } // Дата, может быть null

    [Column("version")]
    public string? Version { get; set; } // Версия, может быть null

    public string? LocalPath { get; set; } // локальный путь, может быть null

    public DateTime? ModifiedAt { get; set; } // Дата, может быть null

    public int? Words { get; set; } // слова, может быть null

    [Column("token")]
    public string? token { get; set; }

    public string? Link2 { get; set; }

    public int DocStatus { get; set; }

    public int? OldId { get; set; }

    public int? MId { get; set; }

    // Навигационные свойства для связи с другими таблицами
    //public virtual NModel? Model { get; set; } // Связь с моделью
    public virtual DocumentType? DocumentType { get; set; } // Связь с типом документа

 
}
