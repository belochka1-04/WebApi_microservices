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
    public int? Cnt { get; set; }

    [Column("BrandId")]
    public int? BrandId { get; set; }

    [Column("SiteId")]
    public int? SiteId { get; set; }

    // навигация к моделям (ModelTb.BrandModel уже есть)
    public virtual ICollection<ModelTb> Models { get; set; } = new List<ModelTb>();

    // навигация к Site
    [ForeignKey(nameof(SiteId))]
    public virtual Site? Site { get; set; }

    // навигация к Brand
    [ForeignKey(nameof(BrandId))]
    public virtual Brand? Brand { get; set; }
}
