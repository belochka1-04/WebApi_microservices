using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("BrandModel")]
public partial class BrandModel
{
    public int Id { get; set; }

    [Column("Code")]
    public string Brand { get; set; } = string.Empty;
    [Column("Cnt")]
    public int Cnt { get; set; }
    [Column("BrandId")]
    public int? BrandId { get; set; }
    [Column("SiteId")]
    public int? SiteId { get; set; }
    
    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
