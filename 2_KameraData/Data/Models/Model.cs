using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KameraData.Data.Models;
[Table("models")]
public partial class Model
{
    public int Id { get; set; }

    [Column("model")]
    public string? model { get; set; } = string.Empty;
    [Column("cleaned_model")]
    public string? CleanedModel { get; set; } = null!;
    [Column("source_id")]
    public int? SourceId { get; set; }
    [Column("brand")]
    public string? Brand { get; set; }
    [Column("confidence")]
    public string? Confidence { get; set; } = null!;
    [Column("categories_id")]
    public int? CategoriesId { get; set; }

    public string? FileTitle { get; set; }

    [JsonIgnore]
    public string? FileName { get; set; }

    public string? WebLink { get; set; }

    //[JsonIgnore]
    //public string? OldWeblink { get; set; } = null!;
    [Column("local_path")]
    public string? LocalPath { get; set; }
    [Column("date_model")]
    public DateTime? DateModel { get; set; }
    [Column("version")]
    public string? Version { get; set; }

    public string? Token { get; set; }

    public virtual ICollection<JobDoc> JobDocs { get; set; } = new List<JobDoc>();
}
