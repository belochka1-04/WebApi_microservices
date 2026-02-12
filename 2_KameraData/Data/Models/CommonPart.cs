using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    [Table("Common_parts")]
    public partial class CommonPart
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("model_id")]
        public int ModelId { get; set; }

        [Column("parts_and_replaces_id")]
        public int PartsAndReplacesId { get; set; }

        [ForeignKey("ModelId")]
        public virtual Model Model { get; set; } = null!;

        [ForeignKey("PartsAndReplacesId")]
        public virtual PartsAndReplace PartsAndReplaces { get; set; } = null!;
    }
}
