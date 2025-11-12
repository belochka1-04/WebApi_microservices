using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KameraData.Data.Models
{
    public partial class Parts
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// [part_number] SKU детали
        /// </summary>
        public string? PartNumber { get; set; } = string.Empty;

        /// <summary>
        /// [Description] Название детали
        /// </summary>
        public string? PartName { get; set; } = string.Empty;

        /// <summary>
        /// [id_model] Внешний ключ на ID из <b>models</b>
        /// </summary>
        [NotMapped]
        //[JsonIgnore]
        public int IdModel {get; set; } = 0;

        public string? Link { get; set; }

        public string? Note { get; set; }

        public int? SiteId { get; set; }

        public virtual Site? Site { get; set; }

        //public virtual ModelPart PartModelV { get; set; }
}
}
