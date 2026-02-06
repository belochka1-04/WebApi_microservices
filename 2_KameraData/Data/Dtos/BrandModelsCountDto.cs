using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    /// <summary>
    /// DTO для количества моделей бренда
    /// </summary>
    public class BrandModelsCountDto
    {
        public int BrandId { get; set; }
        public int ModelsCount { get; set; }
    }
}
