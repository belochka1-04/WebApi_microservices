using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    [Table("Model")]
    [Index("SiteId", Name = "IX_Model")]
    [Index("BrandModelId", Name = "IX_Model_BrandModelId")]
    [Index("CleanedModel", Name = "IX_Model_CleanedModel")]
    [Index("SiteId", Name = "IX_Model_SiteId")]
    [Index("Id", Name = "IX_model_id")]
    public partial class ModelTb
    {
        [Key]
        public int Id { get; set; }

        [StringLength(150)]
        public string? Title { get; set; }

        [StringLength(255)]
        public string? Link { get; set; }

        public string? Description { get; set; }

        public int? BrandModelId { get; set; }

        public int? SiteId { get; set; }

        [Column("token")]
        [StringLength(6)]
        public string? Token { get; set; }

        [Column("cleaned_model")]
        [StringLength(255)]
        public string? CleanedModel { get; set; }

        [Column("Part_counter")]
        public int? PartCounter { get; set; }

        [Column("CP_counter")]
        public int? CpCounter { get; set; }

        public int? LinkState { get; set; }

        [ForeignKey("BrandModelId")]
        public virtual BrandModel? BrandModel { get; set; }

        // без InverseProperty
        public virtual ICollection<CommonPart> CommonParts { get; set; } = new List<CommonPart>();

        // без InverseProperty
        public virtual ICollection<ModelPart> ModelParts { get; set; } = new List<ModelPart>();

        [ForeignKey("SiteId")]
        public virtual Site? Site { get; set; }
    }
}
