using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("Site")]
public partial class Site
{
    public int Id { get; set; }

    [Column("Title")]
    public string Title { get; set; } = string.Empty;
    [Column("Confidence")]
    public string? confidence { get; set; }
    public string? DataTypes { get; set; } = string.Empty;
    public string? FolderPath { get; set; } = string.Empty;
    public string? Link_template { get; set; }

}
