using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    /// <summary>
    /// DTO для создания записи в истории ссылок модели.
    /// </summary>
    public class CreateModelLinkHistoryRequest
    {
        /// <summary>
        /// ID модели, для которой менялась ссылка.
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Старая ссылка.
        /// </summary>
        public string OldLink { get; set; } = null!;

        /// <summary>
        /// Источник изменения (например, "GoogleModelUpdater").
        /// </summary>
        public string? Source { get; set; }
    }
}
