using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class BrandWithCountDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int ModelsCount { get; set; }
    }
}
