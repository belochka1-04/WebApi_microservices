using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    /// <summary>
    /// DTO для обновления ссылки модели.
    /// </summary>
    public class UpdateModelLinkRequest
    {
        /// <summary>
        /// Новая ссылка для модели.
        /// </summary>
        public string NewLink { get; set; } = null!;

        /// <summary>
        /// Источник изменения (для history), например "GoogleModelUpdater".
        /// </summary>
        public string? Source { get; set; }
    }
}
