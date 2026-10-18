using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;

[Table("Model")]
public partial class NModel
{
    public int Id { get; set; }

    [Column("Title")]
    public string? model { get; set; } = string.Empty;

    [Column("cleaned_model")]
    public string? CleanedModel { get; set; }

    [Column("SiteId")]
    public int? SourceId { get; set; }

    [NotMapped]
    public int CategoriesId { get; set; } = 6;

    [NotMapped]
    public string? FileTitle { get; set; }

    [NotMapped]
    public string? FileName { get; set; }

    [Column("BrandModelId")]
    public int? BrandModelId { get; set; }

    [Column("Link")]
    public string? WebLink { get; set; }

    [NotMapped]
    public string? OldWeblink { get; set; }

    [NotMapped]
    public string? LocalPath { get; set; }

    [NotMapped]
    public DateOnly? DateModel { get; set; }

    [NotMapped]
    public string? Version { get; set; }

    [Column("token")]
    public string? Token { get; set; }

    public int? CP_counter { get; set; }

    public virtual BrandModel? Brand { get; set; }

    [ForeignKey("SourceId")]
    public virtual Site? Confidence { get; set; }
}
