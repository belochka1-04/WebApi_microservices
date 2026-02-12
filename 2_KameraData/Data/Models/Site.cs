// Site.cs
using KameraData.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Site")]
public partial class Site
{
    public int Id { get; set; }

    [Column("Title")]
    public string Title { get; set; } = string.Empty;

    [Column("Confidence")]
    public string? Confidence { get; set; }

    [Column("data_types")]
    public string? DataTypes { get; set; }

    [Column("folder_path")]
    public string? FolderPath { get; set; }

    [Column("Link_template")]
    public string? LinkTemplate { get; set; }

    public virtual ICollection<BrandModel> BrandModels { get; set; } = new List<BrandModel>();
}
