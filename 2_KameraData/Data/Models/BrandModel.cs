using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("BrandModel")]
public partial class BrandModel
{
    [Key]
    public int Id { get; set; }

    [Column("Code")]
    public string Code { get; set; } = string.Empty;
    [Column("Cnt")]
    public int Cnt { get; set; }
    [Column("BrandId")]
    public int? BrandId { get; set; }
    [Column("SiteId")]
    public int? SiteId { get; set; }
    
    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

   [InverseProperty("BrandModel")]
    public virtual ICollection<ModelTb> Models { get; set; } = new List<ModelTb>();

    [ForeignKey("SiteId")]
    [InverseProperty("BrandModels")]
    public virtual Site? Site { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("BrandModels")]
    public virtual Brand? Brand { get; set; }  
}
