using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class CreatePartDto
    {
        public string PartNumber { get; set; } = string.Empty;
        public int? UserId { get; set; } 
    }
}
