using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class GetOrCreateBrandModelRequest
    {
        public string BrandTitle { get; set; } = null!;
        public int SiteId { get; set; }
    }
}
